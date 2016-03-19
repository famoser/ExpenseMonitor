<?php
/**
 * Created by PhpStorm.
 * User: Florian Moser
 * Date: 09.01.2016
 * Time: 14:56
 */

namespace famoser\expensemonitor\webpage\models;


class Expense extends BaseGuidModel
{
    public $ExpenseCollectionId;
    public $Description;
    public $CreateTime;
    public $Amount;
}