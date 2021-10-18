using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TaskSort
{
    class Program
    {
        /*
         * Требования:
         * Первыми должны быть отмеченные
         * далее 
         * Далее для каждой переналадки подтянуть задачи
         */
        static void Main(string[] args)
        {
            Task[] tasks = new Task[] {
                new Task(1,1,1, "11"),
                new Task(2,2, 2, "22"),
                new Task(3,3,3, "11"),
                new Task(4,99,4, "44",false,0,true),
                new Task(5,2, 5,"55"),
                new Task(6,4, 6,"44"),
                new Task(7,5, 7,"77",false,0,false,true),
                new Task(9,9, 9, "99"){ prevTaskId =3 },
                new Task(8,3, 6, "444"),
            };
            Console.WriteLine("input:");
            foreach (var task in tasks)
                Console.WriteLine(task);

            Task[] sortedTasks = GetSortedTasks(tasks);

            Console.Read();
        }

        private static Task[] GetSortedTasks(Task[] tasks)
        {
            var workTasks = tasks.Where(task => !task.isAdjust).ToArray();
            Array.Sort(workTasks, (a, b) => a.pos - b.pos);
            Array.Sort(workTasks, (a, b) => b.doing.GetHashCode() - a.doing.GetHashCode());
            Array.Sort(workTasks, (a, b) => b.mustFirst.GetHashCode() - a.mustFirst.GetHashCode());

            Console.WriteLine("sorted input:");
            PrintList(workTasks);
            List<Task> sortedTasks = new List<Task>();
            //Array.Sort(workTasks, (a, b) => 1 - (a.adjust == b.adjust && a.opIndexInBatch == b.opIndexInBatch).GetHashCode());
            foreach (var task in workTasks)
            {
                foreach (var task2 in workTasks)
                    if (task2.adjust == task.adjust && task2.opIndexInBatch == task.opIndexInBatch)
                    {
                        if (!sortedTasks.Contains(task2))
                            sortedTasks.Add(task2);
                    }

                Task[] sortedTasks1 = sortedTasks.ToArray();
                foreach (var sortedTask in sortedTasks1)
                    foreach (var task2 in workTasks)
                        if (task2.batchId == sortedTask.batchId)
                        {
                            if (!sortedTasks1.Contains(task2))
                                sortedTasks.Add(task2);
                        }
                Task[] sortedTasks2 = sortedTasks.ToArray();
                foreach (var sortedTask in sortedTasks2)
                    foreach (var task2 in workTasks)
                        if (task2.prevTaskId == sortedTask.id)
                        {
                            if (!sortedTasks2.Contains(task2))
                                sortedTasks.Add(task2);
                        }
            }
            //foreach (var task in workTasks)
            //{
            //    foreach (var task2 in workTasks)
            //        if (task2.adjust == task.adjust && task2.opIndexInBatch == task.opIndexInBatch)
            //        {
            //            if (!sortedTasks.Contains(task2))
            //                sortedTasks.Add(task2);
            //        }
            //    foreach (var task2 in workTasks)
            //        if (task2.batchId == task.batchId)
            //        {
            //            if (!sortedTasks.Contains(task2))
            //                sortedTasks.Add(task2);
            //        }
            //}
            foreach (var task in sortedTasks)
                task.newPos = sortedTasks.IndexOf(task) + 1;
            Console.WriteLine("sorted output:");
            PrintList(sortedTasks);
            return sortedTasks.ToArray();
        }

        private static void PrintList(IEnumerable<Task> tasks)
        {
            foreach (var task in tasks)
                Console.WriteLine(task);
        }

        private class Task
        {
            public int pos;
            public int id;
            public int newPos;
            public string adjust;
            public int opIndexInBatch;
            public bool doing;
            public bool mustFirst;
            public Task adjustTask;
            public bool isAdjust;
            public int batchId;
            public int prevTaskId;

            public Task(int id, int pos, int batchId, string adjust, bool isAdjust = false, int opIndex = 0, bool mustFirst = false, bool doing = false)
            {
                this.id = id;
                this.pos = pos;
                this.adjust = adjust;
                this.opIndexInBatch = opIndex;
                this.mustFirst = mustFirst;
                this.isAdjust = isAdjust;
                this.doing = doing;
                this.batchId = batchId;
            }
            override public string ToString()
            {
                return String.Format("{0} {1} {2} id{3} mk{4}", pos, newPos, adjust, id, batchId);
            }
        }
    }
}
