using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mahdyar_Library.Classes
{
    /// <summary>
    /// حل برخی مسائل نیازمند تغییرات و افزودن امکاناتی و اپشن های برای انتیتی ها است
    /// مثلا ممکن است یک موجودیت حداقل یک بار نیاز باشد در یک گره موجود باشد
    /// یا مثلا در یک گره بیش از یک موجودیت از یک نوع باید موجود باشد
    /// ممکن است ترتیب موجودیت ها در زمان پردازش مهم باشد
    /// یا ممکنه مثلا بخواهیم از یک موجودیت آرایه ای اولویت را طوری قرار دهیم یا
    /// آیتم هایی را از آن گذینیم که شرایط خاصی داشته باشند و یا
    /// طوری آیتم برگزینیم که در انتخاب موارد بعدی برای پرامیس بودن به مشکل بر نخوریم
    /// 
    /// </summary>
    /// <typeparam name="Entity1"></typeparam>
    /// <typeparam name="Entity2"></typeparam>
    /// <typeparam name="Entity3"></typeparam>
    public class AStar<Entity1, Entity2, Entity3> where Entity1 : new()
    {
        List<int> Tracelist;
        List<int> Counter;
        List<List<int>> results;

        /// <summary>
        /// returns 0 if promissed else must return the entity id
        /// </summary>
        public event Func<TreeNode, int> Evt_Promissed;
        public event Action<TreeNode> Evt_NodeFailure;
        public event Action<TreeNode> Evt_BranchSuccessed;
        public List<Entity1> _Entity1Collection;
        public List<Entity2> _Entity2Collection;
        public List<Entity3> _Entity3Collection;
        public int EntityCount = 3;

        public bool OrderIsImportant = true;
        long _BranchCount = 0;

        public long CurrentBranchNumber
        {
            get
            {
                return GetProgressOf(Counter);
            }
        }
        public long BranchCount
        {
            get
            {
                return _BranchCount;
            }
        }
        public List<TreeNode> BrachNodes
        {
            get
            {
                List<TreeNode> ret = new List<TreeNode>();

                ret.Add(new TreeNode()
                {
                    entity1 = _Entity1Collection[Counter[0]]
               ,
                    entity2 = _Entity2Collection[Counter[1]]
               ,
                    entity3 = _Entity3Collection[Counter[2]]
                });

                return ret;
            }
        }
        public List<List<TreeNode>> ResultsBranch
        {
            get
            {
                List<List<TreeNode>> ret = new List<List<TreeNode>>();
                List<TreeNode> ret2 = new List<TreeNode>();

                for (int i = 0; i < results.Count; i++)
                {
                    ret2.Add(new TreeNode()
                    {
                        entity1 = _Entity1Collection[results[i][0]]
                        ,
                        entity2 = _Entity2Collection[results[i][1]]
                        ,
                        entity3 = _Entity3Collection[results[i][2]]
                    });
                    ret.Add(ret2);
                }
                return ret;
            }
        }


        public void Init(List<Entity1> entity1, List<Entity2> entity2, List<Entity3> entity3)
        {
            _Entity1Collection = entity1;
            _Entity2Collection = entity2;
            _Entity3Collection = entity3;

            Tracelist = new List<int>();
            Counter = new List<int>();
            results = new List<List<int>>();

            Tracelist.Add(entity1.Count);
            Tracelist.Add(entity2.Count);
            Tracelist.Add(entity3.Count);

            Counter.Add(0);
            Counter.Add(0);
            Counter.Add(0);

            _BranchCount = GetProgressOf(Tracelist); ;
        }

        public void SearchTree()
        {
            int ret = -1;
            do
            {
                var currentnode = new TreeNode()
                {
                    entity1 = _Entity1Collection[Counter[0]]
                    ,
                    entity2 = _Entity2Collection[Counter[1]]
                    ,
                    entity3 = _Entity3Collection[Counter[2]]
                };

                ret = Evt_Promissed(currentnode) - 1;
                
                if (ret != -1)
                { Evt_NodeFailure?.Invoke(currentnode); if (!Jump(ret)) break; else continue; }
                else
                {
                    Evt_BranchSuccessed?.Invoke(currentnode);

                    List<int> res = new List<int>();
                    res.Add(Counter[0]);
                    res.Add(Counter[1]);
                    res.Add(Counter[2]);
                    results.Add(res);
                }

                if (!OneStepForward()) break;
                // if (Arrived_to_bound()) break;
            }
            while (true);
        }

        public Int64 GetProgressOf(List<int> L)
        {
            Int64 Progress = 0;

            for (int i = 0; i < L.Count; i++)
            {
                Progress += (get_value(i, L));
            }

            return Progress;
        }
        Int64 get_value(int index, List<int> L)
        {
            Int64 r = L[index];
            for (int i = index + 1; i < Tracelist.Count; i++)
                r *= (Tracelist[i]);
            return r;
        }

        bool OneStepForward()
        {
            int j = Tracelist.Count - 1;
            while (j >= 0)
            {
                if (Counter[j] < Tracelist[j] - 1)
                {
                    Counter[j]++;
                    return true;
                }
                else
                {
                    j--;
                    for (int k = j + 1; k < Tracelist.Count; k++)
                        Counter[k] = 0;
                }
            }
            return false;
        }
        bool Jump(int index)
        {
            while (index >= 0)
                if (Counter[index] < Tracelist[index] - 1)
                {
                    Counter[index]++;
                    return true;
                }
                else
                {
                    index--;
                    for (int k = index + 1; k < Tracelist.Count; k++)
                        Counter[k] = 0;
                }
            return false;
        }

        public class TreeNode
        {
            public Entity1 entity1;
            public Entity2 entity2;
            public Entity3 entity3;
        }
    }

    public class AStar<Entity1, Entity2, Entity3,Entity4> where Entity1 : new()
    {
        List<int> Tracelist;
        List<int> Counter;
        List<List<int>> results;

        /// <summary>
        /// returns 0 if promissed else must return the entity id
        /// </summary>
        public event Func<TreeNode, int> Evt_Promissed;
        public event Action<TreeNode> Evt_NodeFailure;
        public event Action<TreeNode> Evt_BranchSuccessed;
        public List<Entity1> _Entity1Collection;
        public List<Entity2> _Entity2Collection;
        public List<Entity3> _Entity3Collection;
        public List<Entity4> _Entity4Collection;

        public int EntityCount = 4;

        public bool OrderIsImportant = true;
        long _BranchCount = 0;

        public long CurrentBranchNumber
        {
            get
            {
                return GetProgressOf(Counter);
            }
        }
        public long BranchCount
        {
            get
            {
                return _BranchCount;
            }
        }
        public List<TreeNode> BrachNodes
        {
            get
            {
                List<TreeNode> ret = new List<TreeNode>();

                ret.Add(new TreeNode()
                {
                    entity1 = _Entity1Collection[Counter[0]]
               ,
                    entity2 = _Entity2Collection[Counter[1]]
               ,
                    entity3 = _Entity3Collection[Counter[2]]
                ,
                    entity4 = _Entity4Collection[Counter[3]]
                });

                return ret;
            }
        }
        public List<List<TreeNode>> ResultsBranch
        {
            get
            {
                List<List<TreeNode>> ret = new List<List<TreeNode>>();
                List<TreeNode> ret2 = new List<TreeNode>();

                for (int i = 0; i < results.Count; i++)
                {
                    ret2.Add(new TreeNode()
                    {
                        entity1 = _Entity1Collection[results[i][0]]
                        ,
                        entity2 = _Entity2Collection[results[i][1]]
                        ,
                        entity3 = _Entity3Collection[results[i][2]]
                        ,
                        entity4 = _Entity4Collection[results[i][3]]

                    });
                    ret.Add(ret2);
                }
                return ret;
            }
        }


        public void Init(List<Entity1> entity1, List<Entity2> entity2, List<Entity3> entity3,List<Entity4> entity4)
        {
            _Entity1Collection = entity1;
            _Entity2Collection = entity2;
            _Entity3Collection = entity3;
            _Entity4Collection = entity4;

            Tracelist = new List<int>();
            Counter = new List<int>();
            results = new List<List<int>>();

            Tracelist.Add(entity1.Count);
            Tracelist.Add(entity2.Count);
            Tracelist.Add(entity3.Count);
            Tracelist.Add(entity4.Count);

            Counter.Add(0);
            Counter.Add(0);
            Counter.Add(0);
            Counter.Add(0);

            _BranchCount = GetProgressOf(Tracelist); ;
        }

        public void SearchTree()
        {
            int ret = -1;
            do
            {
                var currentnode = new TreeNode()
                {
                    entity1 = _Entity1Collection[Counter[0]]
                    ,
                    entity2 = _Entity2Collection[Counter[1]]
                    ,
                    entity3 = _Entity3Collection[Counter[2]]
                    ,
                    entity4 = _Entity4Collection[Counter[3]]
                };

                ret = Evt_Promissed(currentnode) - 1;

                if (ret != -1)
                { Evt_NodeFailure?.Invoke(currentnode); if (!Jump(ret)) break; else continue; }
                else
                {
                    Evt_BranchSuccessed?.Invoke(currentnode);

                    List<int> res = new List<int>();
                    res.Add(Counter[0]);
                    res.Add(Counter[1]);
                    res.Add(Counter[2]);
                    res.Add(Counter[3]);

                    results.Add(res);
                }

                if (!OneStepForward()) break;
                // if (Arrived_to_bound()) break;
            }
            while (true);
        }

        public Int64 GetProgressOf(List<int> L)
        {
            Int64 Progress = 0;

            for (int i = 0; i < L.Count; i++)
            {
                Progress += (get_value(i, L));
            }

            return Progress;
        }
        Int64 get_value(int index, List<int> L)
        {
            Int64 r = L[index];
            for (int i = index + 1; i < Tracelist.Count; i++)
                r *= (Tracelist[i]);
            return r;
        }

        bool OneStepForward()
        {
            int j = Tracelist.Count - 1;
            while (j >= 0)
            {
                if (Counter[j] < Tracelist[j] - 1)
                {
                    Counter[j]++;
                    return true;
                }
                else
                {
                    j--;
                    for (int k = j + 1; k < Tracelist.Count; k++)
                        Counter[k] = 0;
                }
            }
            return false;
        }
        bool Jump(int index)
        {
            while (index >= 0)
                if (Counter[index] < Tracelist[index] - 1)
                {
                    Counter[index]++;
                    return true;
                }
                else
                {
                    index--;
                    for (int k = index + 1; k < Tracelist.Count; k++)
                        Counter[k] = 0;
                }
            return false;
        }

        public class TreeNode
        {
            public Entity1 entity1;
            public Entity2 entity2;
            public Entity3 entity3;
            public Entity4 entity4;
        }
    }

    public class AStar<Entity1, Entity2, Entity3, Entity4, Entity5> where Entity1 : new()
    {
        List<int> Tracelist;
        List<int> Counter;
        List<List<int>> results;

        /// <summary>
        /// returns 0 if promissed else must return the entity id
        /// </summary>
        public event Func<TreeNode, int> Evt_Promissed;
        public event Action<TreeNode> Evt_NodeFailure;
        public event Action<TreeNode> Evt_BranchSuccessed;
        public List<Entity1> _Entity1Collection;
        public List<Entity2> _Entity2Collection;
        public List<Entity3> _Entity3Collection;
        public List<Entity4> _Entity4Collection;
        public List<Entity5> _Entity5Collection;

        public int EntityCount = 5;

        public bool OrderIsImportant = true;
        long _BranchCount = 0;

        public long CurrentBranchNumber
        {
            get
            {
                return GetProgressOf(Counter);
            }
        }
        public long BranchCount
        {
            get
            {
                return _BranchCount;
            }
        }
        public List<TreeNode> BrachNodes
        {
            get
            {
                List<TreeNode> ret = new List<TreeNode>();

                ret.Add(new TreeNode()
                {
                    entity1 = _Entity1Collection[Counter[0]]
               ,
                    entity2 = _Entity2Collection[Counter[1]]
               ,
                    entity3 = _Entity3Collection[Counter[2]]
               ,
                    entity4 = _Entity4Collection[Counter[3]]  
               ,
                    entity5 = _Entity5Collection[Counter[4]]
                });

                return ret;
            }
        }
        public List<List<TreeNode>> ResultsBranch
        {
            get
            {
                List<List<TreeNode>> ret = new List<List<TreeNode>>();
                List<TreeNode> ret2 = new List<TreeNode>();

                for (int i = 0; i < results.Count; i++)
                {
                    ret2.Add(new TreeNode()
                    {
                        entity1 = _Entity1Collection[results[i][0]]
                        ,
                        entity2 = _Entity2Collection[results[i][1]]
                        ,
                        entity3 = _Entity3Collection[results[i][2]]
                        ,
                        entity4 = _Entity4Collection[results[i][3]]
                        ,
                        entity5 = _Entity5Collection[results[i][4]]
                    });
                    ret.Add(ret2);
                }
                return ret;
            }
        }


        public void Init(List<Entity1> entity1, List<Entity2> entity2, List<Entity3> entity3, List<Entity4> entity4,List<Entity5> entity5)
        {
            _Entity1Collection = entity1;
            _Entity2Collection = entity2;
            _Entity3Collection = entity3;
            _Entity4Collection = entity4;
            _Entity5Collection = entity5;


            Tracelist = new List<int>();
            Counter = new List<int>();
            results = new List<List<int>>();

            Tracelist.Add(entity1.Count);
            Tracelist.Add(entity2.Count);
            Tracelist.Add(entity3.Count);
            Tracelist.Add(entity4.Count);
            Tracelist.Add(entity5.Count);

            Counter.Add(0);
            Counter.Add(0);
            Counter.Add(0);
            Counter.Add(0);
            Counter.Add(0);

            _BranchCount = GetProgressOf(Tracelist);
        }

        public void SearchTree()
        {
            int ret = -1;
            do
            {
                var currentnode = new TreeNode()
                {
                    entity1 = _Entity1Collection[Counter[0]]
                    ,
                    entity2 = _Entity2Collection[Counter[1]]
                    ,
                    entity3 = _Entity3Collection[Counter[2]]
                    ,
                    entity4 = _Entity4Collection[Counter[3]]
                    ,
                    entity5 = _Entity5Collection[Counter[4]]
                };

                ret = Evt_Promissed(currentnode) - 1;

                if (ret != -1)
                { Evt_NodeFailure?.Invoke(currentnode); if (!Jump(ret)) break; else continue; }
                else
                {
                    Evt_BranchSuccessed?.Invoke(currentnode);

                    List<int> res = new List<int>();
                    res.Add(Counter[0]);
                    res.Add(Counter[1]);
                    res.Add(Counter[2]);
                    res.Add(Counter[3]);
                    res.Add(Counter[4]);

                    results.Add(res);
                }

                if (!OneStepForward()) break;
                // if (Arrived_to_bound()) break;
            }
            while (true);
        }

        public Int64 GetProgressOf(List<int> L)
        {
            Int64 Progress = 0;

            for (int i = 0; i < L.Count; i++)
            {
                Progress += (get_value(i, L));
            }

            return Progress;
        }
        Int64 get_value(int index, List<int> L)
        {
            Int64 r = L[index];
            for (int i = index + 1; i < Tracelist.Count; i++)
                r *= (Tracelist[i]);
            return r;
        }

        bool OneStepForward()
        {
            int j = Tracelist.Count - 1;
            while (j >= 0)
            {
                if (Counter[j] < Tracelist[j] - 1)
                {
                    Counter[j]++;
                    return true;
                }
                else
                {
                    j--;
                    for (int k = j + 1; k < Tracelist.Count; k++)
                        Counter[k] = 0;
                }
            }
            return false;
        }
        bool Jump(int index)
        {
            while (index >= 0)
                if (Counter[index] < Tracelist[index] - 1)
                {
                    Counter[index]++;
                    return true;
                }
                else
                {
                    index--;
                    for (int k = index + 1; k < Tracelist.Count; k++)
                        Counter[k] = 0;
                }
            return false;
        }

        public class TreeNode
        {
            public Entity1 entity1;
            public Entity2 entity2;
            public Entity3 entity3;
            public Entity4 entity4;
            public Entity5 entity5;
        }
    }
}
