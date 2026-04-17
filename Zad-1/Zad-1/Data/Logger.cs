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
            catch(IOException ex)
            {
                Console.WriteLine($"I/O error: {ex.Message}");    
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Permission denied: {this._filepath}: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                this._fileLock.Release();
            }
        }
    }
}
