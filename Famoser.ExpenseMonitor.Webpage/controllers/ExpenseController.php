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
use famoser\expensemonitor\webpage\models\entities\ExpenseEntity;
use famoser\expensemonitor\webpage\models\Expense;
use famoser\expensemonitor\webpage\models\communication\ExpenseResponse;
use PDO;

class ExpenseController implements iController
{
    function execute($param, $post)
    {
        try {
            if (count($param) > 0) {
                if ($param[0] == "act") {
                    $obj = json_decode($post["json"]);
                    if ($obj->Action == "delete") {
                        $guids = array();
                        foreach ($obj->Expenses as $note) {
                            $guids[] = $note->Guid;
                        }
                        return ReturnBoolean($this->deleteExpenses($obj->ExpenseTakerGuid, $obj->ExpenseCollectionGuid, $guids));
                    } else if ($obj->Action == "addorupdate") {
                        $newExpenses = array();
                        $updateExpenses = array();
                        $listId = $this->getExpenseCollectionId($obj->ExpenseTakerGuid, $obj->ExpenseCollectionGuid);
                        if ($listId !== false) {
                            foreach ($obj->Expenses as $note) {
                                $existingExpense = GetSingleByCondition("Expenses", array("Guid" => $note->Guid, "ExpenseCollectionId" => $listId));
                                if ($existingExpense == null) {
                                    $newnote = new Expense();
                                    $newnote->ExpenseCollectionId = $listId;
                                    $newnote->Guid = $note->Guid;
                                    $newnote->Description = $note->Description;
                                    $newnote->CreateTime = ConvertToDatabaseDateTime($note->CreateTime);
                                    $newnote->Amount = $note->Amount;
                                    $newExpenses[] = $newnote;
                                } else {
                                    $existingExpense->Description = $note->Description;
                                    $existingExpense->CreateTime = ConvertToDatabaseDateTime($note->CreateTime);
                                    $existingExpense->Amount = $note->Amount;
                                    $updateExpenses[] = $existingExpense;
                                }
                            }
                            $res = InsertAll($newExpenses);
                            $res &= UpdateAll($updateExpenses);
                            return ReturnBoolean($res == "1");
                        }
                        return ReturnBoolean(false);
                    } else if ($obj->Action == "get") {
                        $listId = $this->getExpenseCollectionId($obj->ExpenseTakerGuid, $obj->ExpenseCollectionGuid);
                        if ($listId !== false) {
                            $notes = GetAllByCondition("Expenses", array("ExpenseCollectionId" => $listId));
                            $resp = new ExpenseResponse();
                            foreach ($notes as $note) {
                                $resp->Expenses[] = new ExpenseEntity($note);
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
            return ReturnError("Exception occured: " . $ex->getMessage());
        }
    }

    private function deleteExpenses($noteTakerGuid, $noteCollectionGuid, array $noteGuids)
    {
        $listId = $this->getExpenseCollectionId($noteTakerGuid, $noteCollectionGuid);
        if ($listId !== false) {
            $db = GetDatabaseConnection();
            $noteGuids = array_values($noteGuids);
            $prepareArr = array();
            $keys = array();
            for ($i = 0;  $i < count($noteGuids); $i++) {
                $prepareArr[":guid" . $i] = $noteGuids[$i];
                $keys[] = ":guid" . $i;
            }
            $prepareArr[":ExpenseCollectionId"] = $listId;

            $pdo = $db->prepare("DELETE FROM Expenses WHERE Guid IN (" . implode(",", $keys) . ") AND ExpenseCollectionId = :ExpenseCollectionId");
            return $pdo->execute($prepareArr);
        }
        return false;
    }

    private function getExpenseCollectionId($noteTakerGuid, $noteCollectionGuid)
    {
        $db = GetDatabaseConnection();
        $pdo = $db->prepare(
            "SELECT ExpenseCollections.Id as Id FROM ExpenseCollections
             INNER JOIN ExpenseTakerExpenseCollectionRelations as relation ON relation.ExpenseCollectionId = ExpenseCollections.Id
             INNER JOIN ExpenseTakers as taker ON relation.ExpenseTakerId = taker.Id
             WHERE taker.Guid = :ExpenseTakerGuid AND ExpenseCollections.Guid = :ExpenseCollectionGuid");
        $pdo->bindValue(":ExpenseTakerGuid", $noteTakerGuid);
        $pdo->bindValue(":ExpenseCollectionGuid", $noteCollectionGuid);
        $pdo->execute();

        $rows = $pdo->fetchAll(PDO::FETCH_ASSOC);
        if (count($rows) == 1)
            return $rows[0]["Id"];
        return false;
    }
}