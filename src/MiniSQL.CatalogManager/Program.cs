using MiniSQL.Library.Models;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using MiniSQL.Library.Interfaces;
using System.Diagnostics;
using System.IO;
using MiniSQL.CatalogManager.Controllers;

namespace MiniSQL.CatalogManager
{
    class Program
    {
        static ICatalogManager icatalog = new Catalog("test");
        //check if the program can successfully check the validation of each kind of input statement
        static bool CheckValidation()
        {   //since quitstatement and execfile statement are too simple,we skip them here

            Catalog func = new Catalog("test");

            //select statement
            //3 kinds, each with a true and a false version

            //select only from table without any attribute
            SelectStatement select_only_table_true = new SelectStatement();
            // select_only_table_true.Condition = new Expression();
            select_only_table_true.FromTable = "Student";


            SelectStatement select_only_table_false = new SelectStatement();
            // select_only_table_false.Condition = new Expression();
            select_only_table_false.FromTable = "School";

            bool test1 = func.IsValid(select_only_table_false);
            bool test2 = func.IsValid(select_only_table_true);
            // Console.WriteLine(" Wrong select statement that select only 1 table, expecting false:");
            // Console.WriteLine(test1);
            Debug.Assert(test1 == false);
            // Console.WriteLine(" Right select statement that select only 1 table, expecting true:");
            // Console.WriteLine(test2);
            Debug.Assert(test2 == true);

            //select from table with 1 attribute
            SelectStatement select_with_1_attribute_true = new SelectStatement();
            select_with_1_attribute_true.FromTable = "Student";
            select_with_1_attribute_true.Condition = new Expression();
            select_with_1_attribute_true.Condition.Operator = Operator.AtomVariable;
            select_with_1_attribute_true.Condition.AttributeName = "ID";


            SelectStatement select_with_1_attribute_false = new SelectStatement();
            select_with_1_attribute_false.FromTable = "Student";
            select_with_1_attribute_false.Condition = new Expression();
            select_with_1_attribute_true.Condition.Operator = Operator.AtomVariable;
            select_with_1_attribute_false.Condition.AttributeName = "Gender";

            bool test3 = icatalog.IsValid(select_with_1_attribute_true);
            bool test4 = icatalog.IsValid(select_with_1_attribute_false);
            // Console.WriteLine("Right select statement that select with 1 attribute, expecting true:");
            // Console.WriteLine(test3);
            Debug.Assert(test3 == true);
            // Console.WriteLine("Wrong select statement that select with 1 attribute, expecting false:");
            // Console.WriteLine(test4);
            Debug.Assert(test4 == false);

            // select from table with condition having 3 attribute
            SelectStatement select_with_3_attribute_true = new SelectStatement();
            select_with_3_attribute_true.FromTable = "Student";
            select_with_3_attribute_true.Condition = GetExpression("ID", "Class", "Score");

            SelectStatement select_with_3_attribute_false = new SelectStatement();
            select_with_3_attribute_false.FromTable = "Student";
            select_with_3_attribute_false.Condition = GetExpression("Gender", "Class", "Score");

            bool test5 = icatalog.IsValid(select_with_3_attribute_true);
            bool test6 = icatalog.IsValid(select_with_3_attribute_false);
            // Console.WriteLine("Right select statement that select with 3 attributes, expecting true:");
            // Console.WriteLine(test5);
            Debug.Assert(test5 == true);
            // Console.WriteLine("Wrong select statement that select with 3 attribute, expecting false:");
            // Console.WriteLine(test6);
            Debug.Assert(test6 == false);

            //delete statement
            //3 kinds, each with a true and a false version
            //almost the same as the select statement

            //delete table without any attribute
            DeleteStatement delete_only_table_true = new DeleteStatement();
            delete_only_table_true.TableName = "Student";

            DeleteStatement delete_only_table_false = new DeleteStatement();
            delete_only_table_false.TableName = "School";

            bool tst1 = func.IsValid(delete_only_table_false);
            bool tst2 = func.IsValid(delete_only_table_true);
            // Console.WriteLine("Wrong delete statement that delete only table, expecting false:");
            // Console.WriteLine(tst1);
            Debug.Assert(tst1 == false);
            // Console.WriteLine("Right delete statement that delete only table, expecting true:");
            // Console.WriteLine(tst2);
            Debug.Assert(tst2 == true);

            //delete from table with 1 attribute
            DeleteStatement delete_with_1_attribute_true = new DeleteStatement();
            delete_with_1_attribute_true.TableName = "Student";
            delete_with_1_attribute_true.Condition = new Expression() { Operator = Operator.AtomVariable };
            delete_with_1_attribute_true.Condition.AttributeName = "ID";

            DeleteStatement delete_with_1_attribute_false = new DeleteStatement();
            delete_with_1_attribute_false.Condition = new Expression() { Operator = Operator.AtomVariable };
            delete_with_1_attribute_false.TableName = "Student";
            delete_with_1_attribute_false.Condition.AttributeName = "Gender";

            bool tst3 = icatalog.IsValid(delete_with_1_attribute_true);
            bool tst4 = icatalog.IsValid(delete_with_1_attribute_false);
            // Console.WriteLine("Right delete statement that delete with 1 attribute, expecting true:");
            // Console.WriteLine(tst3);
            Debug.Assert(tst3 == true);
            // Console.WriteLine("Wrong delete statement that delete with 1 attribute, expecting false:");
            // Console.WriteLine(tst4);
            Debug.Assert(tst4 == false);

            //delete from table with 3 attribute
            DeleteStatement delete_with_3_attribute_true = new DeleteStatement();
            delete_with_3_attribute_true.Condition = GetExpression("ID", "Class", "Score");
            delete_with_3_attribute_true.TableName = "Student";

            DeleteStatement delete_with_3_attribute_false = new DeleteStatement();
            delete_with_3_attribute_false.TableName = "Student";
            delete_with_3_attribute_false.Condition = GetExpression("ID", "Gender", "Score");

            bool tst5 = icatalog.IsValid(delete_with_3_attribute_true);
            bool tst6 = icatalog.IsValid(delete_with_3_attribute_false);
            // Console.WriteLine("Right delete statement that delete with 3 attribute, expecting true:");
            // Console.WriteLine(tst5);
            Debug.Assert(tst5 == true);
            // Console.WriteLine("Wrong delete statement that delete with 3 attribute, expecting false:");
            // Console.WriteLine(tst6);
            Debug.Assert(tst6 == false);

            //insert statement
            //3 kinds, only the first is true, others are false

            //all attributes suit
            InsertStatement insert_all_suit = new InsertStatement();
            insert_all_suit.TableName = "Student";

            //make all the values into a list
            List<AtomValue> insert_tmp1 = new List<AtomValue>();
            AtomValue attribute_value1 = new AtomValue();
            attribute_value1.Type = AttributeTypes.Char;
            attribute_value1.StringValue = "111";

            AtomValue attribute_value2 = new AtomValue();
            attribute_value2.Type = AttributeTypes.Char;
            attribute_value2.StringValue = "Lily";

            AtomValue attribute_value3 = new AtomValue();
            attribute_value3.Type = AttributeTypes.Float;
            attribute_value3.FloatValue = 80.5;

            AtomValue attribute_value4 = new AtomValue();
            attribute_value4.Type = AttributeTypes.Int;
            attribute_value4.IntegerValue = 5;

            insert_tmp1.Add(attribute_value1);
            insert_tmp1.Add(attribute_value2);
            insert_tmp1.Add(attribute_value3);
            insert_tmp1.Add(attribute_value4);

            insert_all_suit.Values = insert_tmp1;

            bool insert_test1 = icatalog.IsValid(insert_all_suit);
            // Console.WriteLine("Right insert statement, expecting true:");
            // Console.WriteLine(insert_test1);
            Debug.Assert(insert_test1 == true);


            //doesn't match the number of attributes
            InsertStatement insert_wrong_number = new InsertStatement();
            insert_wrong_number.TableName = "Student";

            //get the value list of only 3 attributes when 4 is the right number
            insert_tmp1.RemoveAt(3);
            insert_wrong_number.Values = insert_tmp1;
            bool insert_test2 = icatalog.IsValid(insert_wrong_number);
            // Console.WriteLine("Wrong insert statement for wrong number of attributes, expecting false:");
            // Console.WriteLine(insert_test2);
            Debug.Assert(insert_test2 == false);

            //doesn't match the type of attributes
            InsertStatement insert_wrong_type = new InsertStatement();
            insert_wrong_type.TableName = "Student";

            //give attribute 4 type string instead of int 
            AtomValue attribute_value5 = new AtomValue();
            attribute_value5.Type = AttributeTypes.Char;
            attribute_value5.StringValue = "hhhh";
            insert_tmp1.Add(attribute_value5);
            insert_wrong_type.Values = insert_tmp1;
            bool insert_test3 = icatalog.IsValid(insert_wrong_type);
            // Console.WriteLine("Wrong insert statement for wrong type of attributes, expecting false:");
            // Console.WriteLine(insert_test3);
            Debug.Assert(insert_test3 == false);



            //drop statement
            //2 kind, try to drop a table or an index, each with a true and a false version

            //drop an existing table, true
            DropStatement drop_table_true = new DropStatement();
            drop_table_true.TargetType = DropTarget.Table;
            drop_table_true.TableName = "Student";
            bool drop_test1 = icatalog.IsValid(drop_table_true);
            // Console.WriteLine("Right drop table statement, expecting true:");
            // Console.WriteLine(drop_test1);
            Debug.Assert(drop_test1 == true);

            //drop a table that doesn't exist, false
            DropStatement drop_table_false = new DropStatement();
            drop_table_false.TargetType = DropTarget.Table;
            drop_table_false.TableName = "School";
            bool drop_test2 = icatalog.IsValid(drop_table_false);
            // Console.WriteLine("Wrong drop table statement, expecting false:");
            // Console.WriteLine(drop_test2);
            Debug.Assert(drop_test2 == false);

            //drop an existing index from the table that doesn't exist, false
            DropStatement drop_index_false1 = new DropStatement();
            drop_index_false1.TargetType = DropTarget.Index;
            drop_index_false1.IndexName = "index_for_student_id";
            drop_index_false1.TableName = "School";
            bool drop_test3 = icatalog.IsValid(drop_index_false1);
            // Console.WriteLine("Wrong drop index statement that drop from the table that doesn't exist, expecting false:");
            // Console.WriteLine(drop_test3);
            Debug.Assert(drop_test3 == false);

            //drop an index that doesn't exist, false
            DropStatement drop_index_false2 = new DropStatement();
            drop_index_false2.TargetType = DropTarget.Index;
            drop_index_false2.IndexName = "index_wrong";
            drop_index_false2.TableName = "Student";
            bool drop_test4 = icatalog.IsValid(drop_index_false2);
            // Console.WriteLine("Wrong drop index statement that drop an unknown index, expecting false:");
            // Console.WriteLine(drop_test4);
            Debug.Assert(drop_test4 == false);

            //drop an index that exits, true
            DropStatement drop_index_true = new DropStatement();
            drop_index_true.TargetType = DropTarget.Index;
            drop_index_true.IndexName = "index_for_student_id";
            drop_index_true.TableName = "Student";
            bool drop_test5 = icatalog.IsValid(drop_index_true);
            // Console.WriteLine("Right drop index statement, expecting true:");
            // Console.WriteLine(drop_test5);
            Debug.Assert(drop_test5 == true);

            //create statement 
            //2 kinds, for index and for table, each with both true and wrong version

            //create a table
            CreateStatement create_table_true = new CreateStatement();
            create_table_true.CreateType = CreateType.Table;
            create_table_true.TableName = "Home";
            bool create_test1 = icatalog.IsValid(create_table_true);
            // Console.WriteLine("Right create table statement, expecting true:");
            // Console.WriteLine(create_test1);
            Debug.Assert(create_test1 == true);

            //create an index, true
            CreateStatement create_index_true = new CreateStatement();
            create_index_true.CreateType = CreateType.Index;
            create_index_true.TableName = "Student";
            create_index_true.IndexName = "second_index_for_student_id";
            create_index_true.AttributeName = "ID";
            bool create_test2 = icatalog.IsValid(create_index_true);
            // Console.WriteLine("Right create index statement, expecting true:");
            // Console.WriteLine(create_test2);
            Debug.Assert(create_test2 == true);

            //create an index that already exists, false
            CreateStatement create_index_false1 = new CreateStatement();
            create_index_false1.CreateType = CreateType.Index;
            create_index_false1.TableName = "Student";
            create_index_false1.IndexName = "index_for_student_id";
            create_index_false1.AttributeName = "ID";
            bool create_test3 = icatalog.IsValid(create_index_false1);
            // Console.WriteLine("Wrong create index statement, expecting false:");
            // Console.WriteLine(create_test3);
            Debug.Assert(create_test3 == false);

            //create an index for an attribute that doesn't eixst
            CreateStatement create_index_false2 = new CreateStatement();
            create_index_false2.CreateType = CreateType.Index;
            create_index_false2.TableName = "Student";
            create_index_false2.IndexName = "index_for_student_home";
            create_index_false2.AttributeName = "Home";
            bool create_test4 = icatalog.IsValid(create_index_false2);
            // Console.WriteLine("Wrong create index statement that is created on an attribute that does not exist, expecting false:");
            // Console.WriteLine(create_test4);
            Debug.Assert(create_test4 == false);

            //create an index for a table that doesn't eixst
            CreateStatement create_index_false3 = new CreateStatement();
            create_index_false3.CreateType = CreateType.Index;
            create_index_false3.TableName = "Home";
            create_index_false3.IndexName = "index_for_student_house";
            create_index_false3.AttributeName = "Street";
            bool create_test5 = icatalog.IsValid(create_index_false3);
            // Console.WriteLine("Wrong create index statement that is created on a table that doesn't exist, expecting false:");
            // Console.WriteLine(create_test5);
            Debug.Assert(create_test5 == false);

            return true;
        }

        static bool CheckCreate()
        {
            //test create statement including index and table 

            //make a create statement for table 
            CreateStatement test_create_table = new CreateStatement();
            test_create_table.CreateType = CreateType.Table;
            test_create_table.TableName = "Student";
            test_create_table.IndexName = "";
            test_create_table.AttributeName = "";

            List<AttributeDeclaration> temp = new List<AttributeDeclaration>();
            AttributeDeclaration student_name = new AttributeDeclaration();
            student_name.AttributeName = "Name";
            student_name.CharLimit = 10;
            student_name.Type = AttributeTypes.Char;
            student_name.IsUnique = false;
            AttributeDeclaration student_score = new AttributeDeclaration();
            student_score.AttributeName = "Score";
            student_score.Type = AttributeTypes.Float;
            student_score.IsUnique = false;
            AttributeDeclaration student_class = new AttributeDeclaration();
            student_class.AttributeName = "Class";
            student_class.Type = AttributeTypes.Int;
            student_class.IsUnique = false;
            AttributeDeclaration student_ID = new AttributeDeclaration();
            student_ID.AttributeName = "ID";
            student_ID.Type = AttributeTypes.Char;
            student_ID.CharLimit = 10;
            student_ID.IsUnique = true;
            temp.Add(student_ID);
            temp.Add(student_name);
            temp.Add(student_score);
            temp.Add(student_class);
            test_create_table.AttributeDeclarations = temp;
            test_create_table.PrimaryKey = "ID";
            //return whether we have successfully created the statement
            bool test1 = icatalog.TryCreateStatement(test_create_table, 0);
            // Console.WriteLine("Create table, expecting true:");
            // Console.WriteLine(test1);
            Debug.Assert(test1 == true);

            //make a create statement for index
            CreateStatement test_create_index = new CreateStatement();
            test_create_index.CreateType = CreateType.Index;
            test_create_index.TableName = "Student";
            test_create_index.IsUnique = true;
            test_create_index.IndexName = "index_for_student_id";
            test_create_index.AttributeName = "ID";
            test_create_index.AttributeDeclarations = new List<AttributeDeclaration>()
                { new AttributeDeclaration() { AttributeName = "priKey", Type = AttributeTypes.Char, CharLimit = 20 } };
            test_create_index.PrimaryKey = "priKey";
            bool test2 = icatalog.TryCreateStatement(test_create_index, 1);
            // Console.WriteLine("Create index1, expecting true:");
            // Console.WriteLine(test2);
            Debug.Assert(test2 == true);

            CreateStatement test_create_index2 = new CreateStatement();
            test_create_index2.CreateType = CreateType.Index;
            test_create_index2.TableName = "Student";
            test_create_index2.IsUnique = true;
            test_create_index2.IndexName = "index2";
            test_create_index2.AttributeName = "ID";
            test_create_index2.AttributeDeclarations = new List<AttributeDeclaration>()
                { new AttributeDeclaration() { AttributeName = "priKey2", Type = AttributeTypes.Char, CharLimit = 20 } };
            test_create_index2.PrimaryKey = "priKey2";
            bool test3 = icatalog.TryCreateStatement(test_create_index2, 7);
            // Console.WriteLine("Create index2, expecting true:");
            // Console.WriteLine(test3);
            Debug.Assert(test3 == true);

            return true;
        }

        static bool CheckDrop()
        {
            //make a drop statement for a index
            //delete this index before delete the table, return true
            DropStatement test_drop_index = new DropStatement();
            test_drop_index.TargetType = DropTarget.Index;
            test_drop_index.IndexName = "index_for_student_id";
            bool test4 = icatalog.TryDropStatement(test_drop_index);
            // Console.WriteLine("Delete Index1, expecting true:");
            // Console.WriteLine(test4);
            Debug.Assert(test4 == true);

            //make a drop statement for a table
            //delete this table and its assotiated indices, return true
            DropStatement test_drop_table = new DropStatement();
            test_drop_table.TargetType = DropTarget.Table;
            test_drop_table.TableName = "Student";
            bool test3 = icatalog.TryDropStatement(test_drop_table);
            // Console.WriteLine("Delete table, expecting true:");
            // Console.WriteLine(test3);
            Debug.Assert(test3 == true);

            //make a drop statement for a index
            //delete this 
            DropStatement test_drop_index2 = new DropStatement();
            test_drop_index2.TargetType = DropTarget.Index;
            test_drop_index2.IndexName = "index2";
            bool test5 = icatalog.TryDropStatement(test_drop_index2);
            // Console.WriteLine("Delete index2 after deleting table, expecting false:");
            // Console.WriteLine(test5);
            Debug.Assert(test5 == false);

            return true;
        }


        //check whether we can successfully return the schema record of a table or an index
        static bool CheckGetSchemaRecord()
        {

            SchemaRecord target_table = icatalog.GetTableSchemaRecord("Student");
            Console.WriteLine("Schema of Table");
            Console.WriteLine(target_table.Name);
            Console.WriteLine(target_table.RootPage);
            Console.WriteLine(target_table.Type);
            Console.WriteLine(target_table.SQL.PrimaryKey);
            Console.WriteLine(target_table.SQL.TableName);
            Console.WriteLine(target_table.SQL.Type);

            SchemaRecord target_index = icatalog.GetIndexSchemaRecord("index_for_student_id");
            Console.WriteLine("Schema of Index");
            Console.WriteLine(target_index.Name);
            Console.WriteLine(target_index.RootPage);
            Console.WriteLine(target_index.Type);
            Console.WriteLine(target_index.SQL.IndexName);
            Console.WriteLine(target_index.SQL.IsUnique);
            Console.WriteLine(target_index.SQL.Type);

            List<SchemaRecord> target_indices = icatalog.GetIndicesSchemaRecord("Student");
            for (int i = 0; i < target_indices.Count; i++)
            {
                Console.WriteLine("Schema of Indices");
                Console.WriteLine(target_indices[i].Name);
                Console.WriteLine(target_indices[i].RootPage);
                Console.WriteLine(target_indices[i].Type);
                Console.WriteLine(target_indices[i].SQL.IndexName);
                Console.WriteLine(target_indices[i].SQL.IsUnique);
                Console.WriteLine(target_indices[i].SQL.Type);
            }
            return true;

        }

        static bool CheckUpdate()
        {
            //update the rootpage of a table
            SchemaRecord table_before = icatalog.GetTableSchemaRecord("Student");
            Console.WriteLine(table_before.RootPage);
            icatalog.TryUpdateSchemaRecord("Student", 2);
            SchemaRecord table_after = icatalog.GetTableSchemaRecord("Student");
            Console.WriteLine(table_after.RootPage);
            Debug.Assert(table_after.RootPage == 2);

            //update the rootpage of an index
            SchemaRecord index_before = icatalog.GetIndexSchemaRecord("index_for_student_id");
            Console.WriteLine(index_before.RootPage);
            icatalog.TryUpdateSchemaRecord("index_for_student_id", 3);
            SchemaRecord index_after = icatalog.GetIndexSchemaRecord("index_for_student_id");
            Console.WriteLine(index_after.RootPage);
            Debug.Assert(index_after.RootPage == 3);

            //try to update the table or index that does not exist
            //will print false
            bool test1 = icatalog.TryUpdateSchemaRecord("HHH", 5);
            bool test2 = icatalog.TryUpdateSchemaRecord("hhh", 6);
            Console.WriteLine(test1);
            Console.WriteLine(test2);
            Debug.Assert(test1 == false);
            Debug.Assert(test2 == false);

            return true;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("[CatalogManager] Start!");
            File.Delete($"./test.indices.dbcatalog");
            File.Delete($"./test.tables.dbcatalog");
            CheckCreate();
            CheckUpdate();
            CheckGetSchemaRecord();
            CheckValidation();
            CheckDrop();
            Console.WriteLine("[CatalogManager] Done!");
        }

        // get test Expression Tree
        private static Expression GetExpression(string name1, string name2, string name3)
        {
            Expression root = new Expression();
            // make condition tree
            root.Operator = Operator.And;
            Expression secondAnd = new Expression() { Operator = Operator.And };
            Expression idOperator = new Expression() { Operator = Operator.Equal };
            Expression idExp = new Expression() { Operator = Operator.AtomVariable, AttributeName = name1 };
            Expression temp1 = new Expression()
            { Operator = Operator.AtomConcreteValue, ConcreteValue = new AtomValue() { Type = AttributeTypes.Int, IntegerValue = 10 } };
            idOperator.LeftOperand = idExp;
            idOperator.RightOperand = temp1;
            Expression classOperator = new Expression() { Operator = Operator.MoreThan };
            Expression classExp = new Expression() { Operator = Operator.AtomVariable, AttributeName = name2 };
            Expression temp2 = new Expression()
            { Operator = Operator.AtomConcreteValue, ConcreteValue = new AtomValue() { Type = AttributeTypes.Int, IntegerValue = 10 } };
            classOperator.LeftOperand = temp2;
            classOperator.RightOperand = classExp;
            Expression scoreOperator = new Expression() { Operator = Operator.LessThanOrEqualTo };
            Expression scoreExp = new Expression() { Operator = Operator.AtomVariable, AttributeName = name3 };
            Expression temp3 = new Expression()
            { Operator = Operator.AtomConcreteValue, ConcreteValue = new AtomValue() { Type = AttributeTypes.Int, IntegerValue = 10 } };
            scoreOperator.LeftOperand = temp3;
            scoreOperator.RightOperand = scoreExp;
            // connect tree trunk
            root.LeftOperand = secondAnd;
            root.RightOperand = scoreOperator;
            secondAnd.LeftOperand = idOperator;
            secondAnd.RightOperand = classOperator;

            return root;
        }
    }
    //to test whether we can 


}
