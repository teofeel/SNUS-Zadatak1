using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zad_1.Data
{
    internal class Logger
    {
        private readonly string _filepath = "log.txt";
        private readonly SemaphoreSlim _fileLock = new SemaphoreSlim(1);

        public async Task WriteLogAsync(string message)
        {
            await this._fileLock.WaitAsync();
            

            try
            {
                await File.AppendAllTextAsync(this._filepath, message);
            }
            finally
            {
                this._fileLock.Release();
            }
        }
    }
}
