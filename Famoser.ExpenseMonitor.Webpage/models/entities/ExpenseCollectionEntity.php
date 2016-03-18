<?php
/**
 * Created by PhpStorm.
 * User: famoser
 * Date: 24.02.2016
 * Time: 17:09
 */

namespace famoser\expensemonitor\webpage\models\entities;


use famoser\expensemonitor\webpage\models\ExpenseCollection;

class ExpenseCollectionEntity extends ExpenseCollection
{
    public function __construct(ExpenseCollection $collection)
    {
        //$this->Id = $ds->Id; Id censored
        $this->Guid = $collection->Guid;
        $this->CreateTime = $collection->CreateTime;
        $this->Name = $collection->Name;
    }
}