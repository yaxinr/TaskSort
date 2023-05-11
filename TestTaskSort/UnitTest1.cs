using ApsTask;

namespace TestTaskSort
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            WorkTask[] tasks = new WorkTask[] {
               /* //new Task(1,1,1,1, "11"){nop="010",prevTaskId = 2 },
                //new Task(2,2,2, 2, "22"){ nop = "030" },
                //new Task(3,3,3,3, "33"),
                new Task(4,4,99,4, "44",false,true),
                //new Task(5,5,2, 5,"55"),
                //new Task(6,6,4, 6,"44"),
                //new Task(7,7,5, 7,"77",false,false,true),
                new Task(9,9,9, 9, "99"){ prevTaskId =3 },
                //new Task(8,6,3, 6, "444"),

                new Task(10,1,11,1, "11", true),
               */
                /* new Task(1,11,2,1, "35"){nop="035" },
                new Task(2,12,3,1, "30", false){nop="030" },
                new Task(3,11,999,1, "35",true){nop="035" },
                */
                /*
                new Task(1,11,2,1, "35",true, false, true){nop="035" },
                new Task(2,12,3,1, "30"){nop="030" },
                new Task(3,11,3,1, "35"){nop="035" },
                */
                new WorkTask(1,11,2,1, "1"){nop="020", prevTaskId = 2 },
                new WorkTask(2,12,2,2, "2"){nop="025", doing = true, prevTaskId=999 },
            };
            Sorting.SortTasks(tasks);
            //Assert.AreEqual(2, sortedTasks.Length);
        }
        [TestMethod]
        public void TestAdjustAndPrev()
        {
            WorkTask task2 = new(id: 2, opId: 12, pos: 2, batchId: 2, adjust: "1") { nop = "020", prevTaskId = 4 };
            var task3 = new WorkTask(id: 3, opId: 31, pos: 2, batchId: 3, adjust: "31") { nop = "025", prevTaskId = 1 };
            var task4 = new WorkTask(id: 4, opId: 32, pos: 99, batchId: 3, adjust: "32") { nop = "030" };
            WorkTask[] tasks = new WorkTask[] {
                new WorkTask(id: 1,opId: 11,pos: 1,batchId: 1, adjust: "1"){ nop="020", doing = true },
                task2,
                task3,
                task4,
            };
            Sorting.SortTasks(tasks);
            Assert.AreEqual(2, task3.NewPos);
            Assert.AreEqual(3, task4.NewPos);
            Assert.AreEqual(4, task2.NewPos);
        }
    }
}