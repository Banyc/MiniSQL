using System;
using System.Collections.Generic;
using MiniSQL.Api.Controllers;
using MiniSQL.Library.Interfaces;
using Xunit;

namespace MiniSQL.Api.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void SelectFromIndexTree()
        {
            DatabaseBuilder builder = new DatabaseBuilder();
            IApi databaseController = new ApiController(builder);
            databaseController.DropDatabase("SelectFromIndexTree");
            databaseController.ChangeContext("SelectFromIndexTree");
            try
            {
                databaseController.Query(
                    @"create table student (
                        sno char(8),
                        sname char(16) unique,
                        sage int,
                        sgender char(1),
                        primary key(sno)
                    );"
                );
                databaseController.Query(
                    "create index stu_name_idx on student(sname);"
                );
                databaseController.Query(
                    "insert into student values('1', 'st1', 21, 'M');"
                );
                databaseController.Query(
                    "insert into student values('2', 'st2', 22, 'M');"
                );
                databaseController.Query(
                    "insert into student values('3', 'st3', 23, 'M');"
                );
                databaseController.Query(
                    "insert into student values('4', 'st4', 24, 'M');"
                );
                List<Library.Models.SelectResult> results = databaseController.Query(
                    "select * from student where sgender = 'M' and sage <= 24 and sage > 21 and sname = 'st3' and sno = '3';"
                );
                Assert.Single(results);
                Assert.Equal(4, results[0].ColumnDeclarations.Count);
                Assert.Single(results[0].Rows);
                Assert.Equal("3", results[0].Rows[0][0].StringValue);
                Assert.Equal("st3", results[0].Rows[0][1].StringValue);
                Assert.Equal(23, results[0].Rows[0][2].IntegerValue);
                Assert.Equal("M", results[0].Rows[0][3].StringValue);
            }
            finally
            {
                databaseController.DropDatabase("SelectFromIndexTree");
            }
        }
    }
}
