<?php
/**
 * Created by PhpStorm.
 * User: Florian Moser
 * Date: 11.01.2016
 * Time: 12:42
 */

namespace famoser\expensemonitor\webpage\models\entities;


use famoser\expensemonitor\webpage\models\Expense;

class ExpenseEntity extends Expense
{
    public function __construct(Expense $note)
    {
        //$this->Id = $ds->Id; Id censored
        $this->Guid = $note->Guid;
        $this->Content = $note->Content;
        $this->CreateTime = $note->CreateTime;
        $this->IsCompleted = $note->IsCompleted;
    }
}