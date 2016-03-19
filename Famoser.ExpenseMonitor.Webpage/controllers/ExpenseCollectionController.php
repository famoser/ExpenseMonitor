<?php
/**
 * Created by PhpStorm.
 * User: Florian Moser
 * Date: 09.01.2016
 * Time: 15:16
 */

namespace famoser\expensemonitor\webpage\controllers;


use famoser\expensemonitor\webpage\core\interfaces\iController;
use function famoser\expensemonitor\webpage\core\responsehelper\ReturnBoolean;
use function famoser\expensemonitor\webpage\core\responsehelper\ReturnError;
use function famoser\expensemonitor\webpage\core\responsehelper\ReturnJson;
use function famoser\expensemonitor\webpage\core\responsehelper\ReturnNotFound;
use function famoser\expensemonitor\webpage\core\validationhelper\ConvertToDatabaseDateTime;
use function famoser\expensemonitor\webpage\core\validationhelper\ValidateGuid;
use famoser\expensemonitor\webpage\models\communication\ExpenseCollectionResponse;
use famoser\expensemonitor\webpage\models\entities\ExpenseCollectionEntity;
use famoser\expensemonitor\webpage\models\entities\ExpenseEntity;
use famoser\expensemonitor\webpage\models\Expense;
use famoser\expensemonitor\webpage\models\communication\ExpenseResponse;
use famoser\expensemonitor\webpage\models\ExpenseCollection;
use famoser\expensemonitor\webpage\models\ExpenseTaker;
use PDO;

class ExpenseCollectionController implements iController
{
    function execute($param, $post)
    {
        try {
            if (count($param) > 0) {
                if ($param[0] == "act") {
                    $obj = json_decode($post["json"]);
                    if ($obj->Action == "delete") {
                        $guids = array();
                        foreach ($obj->ExpenseCollections as $collection) {
                            $guids[] = $collection->Guid;
                        }
                        return ReturnBoolean($this->deleteExpenseCollections($obj->ExpenseTakerGuid, $guids));
                    } else if ($obj->Action == "addorupdate") {
                        $newCollections = array();
                        $updateCollections = array();
                        $taker = $this->tryAddExpenseTaker($obj->ExpenseTakerGuid);
                        if ($taker !== false) {
                            $existingCollections = $this->getExpenseCollections($obj->ExpenseTakerGuid);
                            foreach ($obj->ExpenseCollections as $collection) {
                                $found = false;
                                foreach ($existingCollections as $existingCollection) {
                                    if ($existingCollection->Guid == $collection->Guid)
                                        $found = $existingCollection;
                                }
                                if ($found == false) {
                                    $newCollection = new ExpenseCollection();
                                    $newCollection->Guid = $collection->Guid;
                                    $newCollection->CreateTime = ConvertToDatabaseDateTime($collection->CreateTime);
                                    $newCollection->Name = $collection->Name;
                                    $newCollections[] = $newCollection;
                                } else {
                                    $found->Name = $collection->Name;
                                    $found->CreateTime = ConvertToDatabaseDateTime($collection->CreateTime);
                                    $updateCollections[] = $found;
                                }
                            }
                            return ReturnBoolean($this->addExpenseCollections($obj->ExpenseTakerGuid, $newCollections) && UpdateAll($updateCollections));
                        }
                        return ReturnBoolean(false);
                    } else if ($obj->Action == "get") {
                        $collections = $this->getExpenseCollections($obj->ExpenseTakerGuid);
                        if ($collections !== false) {
                            $resp = new ExpenseCollectionResponse();
                            foreach ($collections as $collection) {
                                $resp->ExpenseCollections[] = new ExpenseCollectionEntity($collection);
                            }
                            return ReturnJson($resp);
                        }
                        return ReturnBoolean(false);
                    } else {
                        return ReturnError(LINK_INVALID);
                    }
                }
            }
            return ReturnError(LINK_INVALID);
        } catch (\Exception $ex) {
            return ReturnError("Exception occured: " . $ex->getMessage(). "\n".$ex->getTraceAsString());
        }
    }

    private function deleteExpenseCollections($noteTakerGuid, array $noteCollectionGuids)
    {
        $collections = $this->getExpenseCollections($noteTakerGuid);
        $noteTakerId = $this->getExpenseTakerId($noteTakerGuid);
        if (count($collections) > 0) {
            $prepareArr = array();
            $keys = array();
            for ($i = 0; $i < count($collections); $i++) {
                if (in_array($collections[$i]->Guid, $noteCollectionGuids)) {
                    $prepareArr[":ExpenseCollectionId" . $i] = $collections[$i]->Id;
                    $keys[] = ":ExpenseCollectionId" . $i;
                }
            }
            $prepareArr[":ExpenseTakerId"] = $noteTakerId;

            $db = GetDatabaseConnection();
            $pdo = $db->prepare("DELETE FROM ExpenseTakerExpenseCollectionRelations WHERE ExpenseCollectionId IN (" . implode(",", $keys) . ") AND ExpenseTakerId = :ExpenseTakerId");
            return $pdo->execute($prepareArr);
            //todo: clean up database
        }
        return false;
    }

    /**
     * @param $noteTakerGuid
     * @return ExpenseCollection[]
     */
    private function getExpenseCollections($noteTakerGuid)
    {
        $db = GetDatabaseConnection();
        $pdo = $db->prepare(
            "SELECT nc.Id as Id, nc.Guid as Guid, nc.Name as Name, nc.CreateTime as CreateTime FROM ExpenseCollections as nc
             INNER JOIN ExpenseTakerExpenseCollectionRelations as relation ON relation.ExpenseCollectionId = nc.Id
             INNER JOIN ExpenseTakers as taker ON relation.ExpenseTakerId = taker.Id
             WHERE taker.Guid = :ExpenseTakerGuid");
        $pdo->bindValue(":ExpenseTakerGuid", $noteTakerGuid);
        $pdo->execute();

        return $pdo->fetchAll(PDO::FETCH_CLASS, GetModelByTable("ExpenseCollections"));
    }

    /**
     * @param $noteTakerGuid
     * @return ExpenseCollection[]
     */
    private function tryAddExpenseTaker($noteTakerGuid)
    {
        $db = GetDatabaseConnection();
        $pdo = $db->prepare(
            "SELECT * FROM ExpenseTakers WHERE ExpenseTakers.Guid = :ExpenseTakerGuid");
        $pdo->bindValue(":ExpenseTakerGuid", $noteTakerGuid);
        $pdo->execute();

        $takers = $pdo->fetchAll(PDO::FETCH_CLASS, GetModelByTable("ExpenseTakers"));
        if (count($takers) == 0) {
            $taker = new ExpenseTaker();
            $taker->Guid = $noteTakerGuid;
            return Insert("ExpenseTakers", $taker);
        }
        return true;
    }

    /**
     * @param $noteTakerGuid
     * @return ExpenseCollection[]
     */
    private function getExpenseTakerId($noteTakerGuid)
    {
        $db = GetDatabaseConnection();
        $pdo = $db->prepare(
            "SELECT Id FROM ExpenseTakers
             WHERE ExpenseTakers.Guid = :ExpenseTakerGuid");
        $pdo->bindValue(":ExpenseTakerGuid", $noteTakerGuid);
        $pdo->execute();

        $rows = $pdo->fetchAll(PDO::FETCH_ASSOC);
        if (count($rows) == 1)
            return $rows[0]["Id"];
        return false;
    }

    /**
     * @param $noteTakerGuid
     * @param ExpenseCollection[] $collections
     */
    private function addExpenseCollections($noteTakerGuid, array $collections)
    {
        $ret = true;
        $noteTakerId = $this->getExpenseTakerId($noteTakerGuid);
        foreach ($collections as $collection) {
            if (Insert("ExpenseCollections", $collection)) {
                $db = GetDatabaseConnection();
                $pdo = $db->prepare(
                    "INSERT INTO ExpenseTakerExpenseCollectionRelations (ExpenseTakerId, ExpenseCollectionId)
                 VALUES (:ExpenseTakerId, :ExpenseCollectionId)");
                $pdo->bindValue(":ExpenseTakerId", $noteTakerId);
                $pdo->bindValue(":ExpenseCollectionId", $collection->Id);
                $ret &= $pdo->execute();
            } else
                $ret = false;
        }
        return $ret;
    }
}