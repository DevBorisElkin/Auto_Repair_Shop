using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class AbstractFactory<T> where T : Entity<T>
    {
        private List<T> entities;

        public Action<T> EntityReleased;

        private int entityProcessDelay;
        static private int iterationsLimit = 20000;

        public AbstractFactory(List<T> entities, int entityProcessDelay)
        {
            this.entityProcessDelay = entityProcessDelay;
            autoResetEvent = new AutoResetEvent(true);
            this.entities = entities;

            EntityReleased += GiveThreadEntity;
        }
        ~AbstractFactory() { EntityReleased -= GiveThreadEntity; }

        void GiveThreadEntity(Entity<T> a)
        {
            autoResetEvent.Set();
        }
        AutoResetEvent autoResetEvent;
        public T GetEntity()
        {
            bool foundFreeEntity = TryGetVacantEntity(out T entity);
            if (foundFreeEntity) return entity;

            autoResetEvent.WaitOne();

            foundFreeEntity = TryGetVacantEntityPersistent(out entity);
            if (foundFreeEntity) return entity;
            else Console.WriteLine("Error, couldn't find free seller althought there's should be at least one");

            return null;
        }

        private bool TryGetVacantEntity(out T entity)
        {
            entity = null;
            foreach (var a in entities)
            {
                if (Interlocked.CompareExchange(ref a._lockFlag, 1, 0) == 0)
                {
                    Monitor.Enter(a);
                    entity = a;
                    return true;
                }
            }
            return false;
        }

        bool TryGetVacantEntityPersistent(out T  entity)
        {
            entity = null;
            int iteration = 1;
            while (entity == null && iteration < iterationsLimit)
            {
                foreach (var a in entities)
                {
                    if (Interlocked.CompareExchange(ref a._lockFlag, 1, 0) == 0)
                    {
                        Monitor.Enter(a);
                        entity = a;

                        Console.WriteLine($"Finished Persistent Entity aquasition on iteration [{iteration}], thread [{Thread.CurrentThread.ManagedThreadId}]");
                        return true;
                    }
                }
                iteration++;

                // this thing makes wonders happen. Without it iterations can get up to 4000.
                // With it still it's not perfect but at least I know that the app is running safe
                int chosenDelay = Math.Clamp(entityProcessDelay, 1, 1000);
                Thread.Sleep(chosenDelay);
            }
            return false;
        }
    }
}
