<?php
/**
 * Created by PhpStorm.
 * User: Florian Moser
 * Date: 11.01.2016
 * Time: 20:17
 */

namespace famoser\expensemonitor\webpage\controllers;


use famoser\expensemonitor\webpage\core\interfaces\iController;
use PDO;

class ApiController implements iController
{
    function execute($param, $post)
    {
        if (count($param) > 0 && $param[0] == "stats") {
            return "Expenses:" . $this->countExpenses() . "<br>ExpenseTakers:" . $this->countExpenseTakers();
        }
        if (count($param) > 0 && $param[0] == "prepare") {
            $this->prepareTable();
        }
        return "Online";
    }

    private function countExpenses()
    {
        $db = GetDatabaseConnection();
        $pdo = $db->prepare("SELECT COUNT(*) FROM Expense");
        $pdo->execute();

        return $pdo->fetch(PDO::FETCH_NUM)[0];
    }

    private function countExpenseTakers()
    {
        $db = GetDatabaseConnection();
        $pdo = $db->prepare("SELECT COUNT(*) FROM ExpenseTaker");
        $pdo->execute();

        return $pdo->fetch(PDO::FETCH_NUM)[0];
    }

    private function prepareTable()
    {
        $db = GetDatabaseConnection();
        $sql = file_get_contents($_SERVER["DOCUMENT_ROOT"] . "/scripts.sql");
        $statements = explode(";", $sql);
        foreach ($statements as $statement) {
            try {
                $pdo = $db->prepare($statement);
                $pdo->execute();
            } catch (\Exception $ex) {
                echo "<p>" . $ex->getMessage() . "</p>";
            }
        }

        echo "<p>Script executed!</p>";
    }
}