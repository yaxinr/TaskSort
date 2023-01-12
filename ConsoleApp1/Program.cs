using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                new Task(1,11,2,1, "1"){nop="020", prevTaskId = 2 },
                new Task(2,12,2,2, "2"){nop="025", doing = true, prevTaskId=999 },
            };
            PrintList(tasks, "input:");

            Task[] sortedTasks = GetSortedTasks(tasks);
            PrintList(sortedTasks, "sorted enumerated tasks:");

            Console.Read();
        }

        private static Task[] GetSortedTasks(Task[] tasks)
        {
            var workTasks = tasks.Where(task => !task.isAdjust).ToArray();
            foreach (var g in workTasks.GroupBy(t => t.batchId))
                if (g.Count() > 1)
                {
                    var ops = g.OrderBy(t => t.nop).ThenBy(t => t.Pos).ToArray();
                    for (int i = 0; i < ops.Length; i++)
                    {
                        Task task = ops[i];
                        task.opIndexInBatch = i;
                        if (i > 0 && task.Pos <= ops[i - 1].Pos)
                            task.newPos = ops[i - 1].Pos + 1;
                    }
                }

            PrintList(workTasks, "sorted input:");
            List<Task> sortedTasks = new List<Task>();
            var tasksById = workTasks.ToDictionary(t => t.id);

            foreach (var task in workTasks)
                if (task.prevTaskId > 0)
                {
                    if (tasksById.TryGetValue(task.prevTaskId, out Task prevTask))
                        task.newPos = prevTask.newPos + 1;
                }

            foreach (var task in workTasks.Where(t => t.mustFirst))
                AddTask(task);
            PrintList(sortedTasks, "sorted must first tasks:");
            foreach (var task in workTasks.Where(t => t.doing))
                AddTask(task);
            PrintList(sortedTasks, "sorted doing tasks:");
            foreach (var task in workTasks.Where(t => t.mustNext))
                AddTask(task);
            foreach (var task in workTasks.OrderBy(t => t.newPos))
                AddTask(task);

            for (int i = 0; i < sortedTasks.Count; i++)
                sortedTasks[i].newPos = (i + 1) * 10 + 1;
            foreach (var task in tasks)
                if (task.isAdjust)
                    foreach (var opTask in sortedTasks.Where(t => t.opId == task.opId))
                        task.newPos = opTask.newPos - 1;
            PrintList(sortedTasks, "sorted work tasks:");
            Array.Sort(tasks, (a, b) => a.newPos - b.newPos);
            PrintList(tasks, "sorted tasks:");
            for (int i = 0; i < tasks.Length; i++)
                tasks[i].newPos = i + 1;
            return tasks;

            void AddRelatedTasks(Task task)
            {
                foreach (var task2 in workTasks)
                    if (task2.adjust == task.adjust)
                    {
                        if (!sortedTasks.Contains(task2))
                            sortedTasks.Add(task2);
                    }
                foreach (var task2 in workTasks)
                    if (task2.batchId == task.batchId)
                    {
                        if (!sortedTasks.Contains(task2))
                            sortedTasks.Add(task2);
                    }
                foreach (var task2 in workTasks)
                    if (task2.prevTaskId == task.id)
                    {
                        if (!sortedTasks.Contains(task2))
                            sortedTasks.Add(task2);
                    }
            }

            void AddTask(Task task)
            {
                if (!sortedTasks.Contains(task))
                {
                    sortedTasks.Add(task);
                    AddRelatedTasks(task);
                }
            }
        }

        private static void PrintList(IEnumerable<Task> tasks, string label)
        {
            Console.WriteLine(label);
            foreach (var task in tasks)
                Console.WriteLine(task);
        }

        private class Task
        {
            private int pos;
            public int Pos => pos;
            public int id;
            public int opId;
            public int newPos;
            public string adjust;
            public int opIndexInBatch;
            public bool doing;
            public bool mustFirst;
            public bool mustNext;
            public bool isAdjust;
            public int batchId;
            public string nop;
            public int prevTaskId;

            public Task(int id, int opId, int pos, int batchId, string adjust, bool isAdjust = false, bool mustFirst = false, bool doing = false)
            {
                this.id = id;
                this.opId = opId;
                this.pos = pos;
                newPos = pos;
                this.adjust = adjust;
                this.mustFirst = mustFirst;
                this.isAdjust = isAdjust;
                this.doing = doing;
                this.batchId = batchId;
            }
            override public string ToString() => $"id={id} {pos}->{newPos} {adjust}  mk{batchId} nop{nop} {(doing ? "doing" : "")} {(mustFirst ? "mustFirst" : "")} {(isAdjust ? "adjust" : "")} mks{opId} prev={prevTaskId}";
        }
    }
}
