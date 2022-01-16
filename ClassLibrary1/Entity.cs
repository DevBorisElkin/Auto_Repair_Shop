using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class Entity<T> where T: Entity<T>
    {
        public AbstractFactory<T> factory;
        public int entityId;

        public int _lockFlag = 0;

        public Entity(AbstractFactory<T> factory, int entityId)
        {
            this.factory = factory;
            this.entityId = entityId;
        }

        public void ReleaseTheEntity()
        {
            Interlocked.Decrement(ref _lockFlag);
            Monitor.Exit(this);
            factory.EntityReleased?.Invoke((T) this);
        }
    }
}
