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
            return "Expenses:".$this->countExpenses()."<br>ExpenseTakers:".$this->countExpenseTakers();
        }
        if (count($param) > 0 && $param[0] == "prepare") {
            return "Funktion not implemented yet";//$this->prepareTable();
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

    /*
    private function prepareTable()
    {
        $db = GetDatabaseConnection();
        $pdo = $db->prepare("select COUNT(*) from sqlite_master");
        $pdo->execute();

        if ($pdo->fetch(PDO::FETCH_NUM)[0] == 0) {
            $pdo = $db->prepare("
CREATE TABLE Expenses
(Id INTEGER PRIMARY KEY AUTOINCREMENT,
Guid varchar(255),
UserGuid varchar(255),
Content text,
CreateTime datetime,
IsCompleted bool)");
            $pdo->execute();

            $pdo = $db->prepare("select COUNT(*) from sqlite_master");
            $pdo->execute();

            if ($pdo->fetch(PDO::FETCH_NUM)[0] === 2) {
                return "Table created!";
            } else
                return "Table creation failed";
        }
        return "Table already ready";
    }*/
}