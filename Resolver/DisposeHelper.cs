namespace Resolver
{
    using System;
    using System.Collections.Generic;

    public static class DisposeHelper
    {
        public static void Dispose(IDisposable disposable)
        {
            disposable?.Dispose();
        }

        public static void ClearCollection<T>(IList<T> collection)
        {
            collection?.Clear();
        }

        public static void DisposeCollection<T>(IList<T> collection) where T : IDisposable
        {
            foreach (var item in collection)
            {
                item.Dispose();
            }

            ClearCollection(collection);
        }
    }
}