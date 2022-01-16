﻿using System;
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

        public AbstractFactory(int amountOfEntities, int entityProcessDelay)
        {
            this.entityProcessDelay = entityProcessDelay;
            autoResetEvent = new AutoResetEvent(true);
            entities = new List<T>();
            for (int i = 1; i <= amountOfEntities; i++)
                entities.Add((T) new Entity<T>(this, i));

            EntityReleased += GiveThreadEntity;
        }
        ~AbstractFactory() { EntityReleased -= GiveThreadEntity; }

        void GiveThreadEntity(Entity<T> a)
        {
            autoResetEvent.Set();
        }
        AutoResetEvent autoResetEvent;
        public Entity<T> GetEntity()
        {
            bool foundFreeEntity = TryGetVacantEntity(out Entity<T> entity);
            if (foundFreeEntity) return entity;

            autoResetEvent.WaitOne();

            foundFreeEntity = TryGetVacantEntityPersistent(out entity);
            if (foundFreeEntity) return entity;
            else Console.WriteLine("Error, couldn't find free seller althought there's should be at least one");

            // just error handling
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("____");
            sb.AppendLine($"Found no sellers, thread id[{Thread.CurrentThread.ManagedThreadId}]");
            foreach (var a in entities)
                sb.AppendLine($"seller lock flag: [{a._lockFlag}]");
            sb.AppendLine("____");
            Console.WriteLine(sb);

            return null;
        }

        private bool TryGetVacantEntity(out Entity<T> entity)
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

        bool TryGetVacantEntityPersistent(out Entity<T> entity)
        {
            entity = null;
            int iteration = 1;
            while (entity == null)
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
