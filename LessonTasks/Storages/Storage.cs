using System;

namespace LessonTasks.Storages
{
    internal abstract class Storage : ICloneable
    {
        public abstract object Clone();
        internal abstract bool Add(string fileName, int fileSize);
    }
}