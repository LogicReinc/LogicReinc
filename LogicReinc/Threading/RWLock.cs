using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LogicReinc.Threading
{
    public class RWLock
    {
        private ReaderWriterLockSlim locker = new ReaderWriterLockSlim();

        public ReaderWriterLockSlim Lock => locker;

        public void ReadLock(Action action)
        {
            locker.EnterReadLock();
            try
            {
                action();
            }
            finally
            {
                locker.ExitReadLock();
            }
        }

        public T ReadLock<T>(Func<T> action)
        {
            locker.EnterReadLock();
            try
            {
                return action();
            }
            finally
            {
                locker.ExitReadLock();
            }
        }

        public void UReadLock(Action<ReaderWriterLockSlim> action)
        {
            locker.EnterUpgradeableReadLock();
            try
            {
                action(locker);
            }
            finally
            {
                locker.ExitUpgradeableReadLock();
            }
        }

        public T UReadLock<T>(Func<ReaderWriterLockSlim, T> action)
        {
            locker.EnterUpgradeableReadLock();
            try
            {
                return action(locker);
            }
            finally
            {
                locker.ExitUpgradeableReadLock();
            }
        }

        public void WriteLock(Action action)
        {
            locker.EnterWriteLock();
            try
            {
                action();
            }
            finally
            {
                locker.ExitWriteLock();
            }
        }
        public T WriteLock<T>(Func<T> action)
        {
            locker.EnterWriteLock();
            try
            {
                return action();
            }
            finally
            {
                locker.ExitWriteLock();
            }
        }
    }
}
