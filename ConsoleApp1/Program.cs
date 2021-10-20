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
                new Task(1,1,1,1, "11"){nop="010" },
                new Task(2,2,2, 2, "22"),
                new Task(3,3,3,3, "11"),
                new Task(4,4,99,4, "44",false,true),
                new Task(5,5,2, 5,"55"),
                new Task(6,6,4, 6,"44"),
                new Task(7,7,5, 7,"77",false,false,true),
                new Task(9,9,9, 9, "99"){ prevTaskId =3 },
                new Task(8,6,3, 6, "444"),

                new Task(10,1,11,1, "11", true),
                new Task(11,11,11,1, "11"){nop="005" },
                new Task(12,12,12,12, "12"){nop="010" },
                new Task(13,13,13,12, "13"){nop="005" },
            };
            Console.WriteLine("input:");
            PrintList(tasks);

            Task[] sortedTasks = GetSortedTasks(tasks);

            Console.Read();
        }

        private static Task[] GetSortedTasks(Task[] tasks)
        {
            var workTasks = tasks.Where(task => !task.isAdjust).ToArray();
            foreach (var g in workTasks.GroupBy(t => t.batchId))
                if (g.Count() > 1)
                {
                    var ops = g.OrderBy(t => t.nop).ThenBy(t => t.pos).ToArray();
                    for (int i = 0; i < ops.Length; i++)
                    {
                        Task task = ops[i];
                        task.opIndexInBatch = i;
                        if (i > 0 && task.pos <= ops[i - 1].pos)
                            task.pos = ops[i - 1].pos + 1;
                    }
                }
            Array.Sort(workTasks, (a, b) => a.pos - b.pos);
            Array.Sort(workTasks, (a, b) => b.doing.GetHashCode() - a.doing.GetHashCode());
            Array.Sort(workTasks, (a, b) => b.mustFirst.GetHashCode() - a.mustFirst.GetHashCode());

            Console.WriteLine("sorted input:");
            PrintList(workTasks);
            List<Task> sortedTasks = new List<Task>();
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
            for (int i = 0; i < sortedTasks.Count; i++)
                sortedTasks[i].newPos = (i + 1) * 10 + 1;
            foreach (var task in tasks)
                if (task.isAdjust)
                    foreach (var opTask in sortedTasks.Where(t => t.opId == task.opId))
                        task.newPos = opTask.newPos - 1;
            Console.WriteLine("sorted work tasks:");
            PrintList(sortedTasks);
            Array.Sort(tasks, (a, b) => a.newPos - b.newPos);
            Console.WriteLine("sorted tasks:");
            PrintList(tasks);
            for (int i = 0; i < tasks.Length; i++)
                tasks[i].newPos = i + 1;
            Console.WriteLine("sorted enumerated tasks:");
            PrintList(tasks);
            return tasks;
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
            public int opId;
            public int newPos;
            public string adjust;
            public int opIndexInBatch;
            public bool doing;
            public bool mustFirst;
            public bool isAdjust;
            public int batchId;
            public string nop;
            public int prevTaskId;

            public Task(int id, int opId, int pos, int batchId, string adjust, bool isAdjust = false, bool mustFirst = false, bool doing = false)
            {
                this.id = id;
                this.opId = opId;
                this.pos = pos;
                this.adjust = adjust;
                this.mustFirst = mustFirst;
                this.isAdjust = isAdjust;
                this.doing = doing;
                this.batchId = batchId;
            }
            override public string ToString()
            {
                return String.Format("{0}->{1} {2} id{3} mk{4} {5} {6} nop{7} {8} mks{9}",
                    pos, newPos, adjust, id, batchId, doing ? "doing" : "", mustFirst ? "mustFirst" : "", nop, isAdjust ? "adjust" : "", opId);
            }
        }
    }
}
