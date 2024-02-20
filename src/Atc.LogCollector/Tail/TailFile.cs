// ReSharper disable InvertIf
namespace Atc.LogCollector.Tail;

public class TailFile : IDisposable
{
    public event Action<TailLine>? LineAdded;

    private readonly object syncLock = new();
    private readonly FileInfo file;
    private readonly FileSystemWatcher? fileWatcher;
    private Thread? fileReadThread;
    private long lastMaxOffset;
    private bool isPaused = true;
    private bool isRunning;

    public TailFile(
        FileInfo file)
    {
        ArgumentNullException.ThrowIfNull(file);

        this.file = file;

        fileWatcher = new FileSystemWatcher(file.Directory!.FullName)
        {
            Filter = file.Name,
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size,
        };

        fileWatcher.Created += OnCreatedOrChanged;
        fileWatcher.Changed += OnCreatedOrChanged;
        fileWatcher.Deleted += OnDeletedOrRenamed;
        fileWatcher.Renamed += OnDeletedOrRenamed;
    }

    public void Start()
    {
        if (fileWatcher is null)
        {
            return;
        }

        lastMaxOffset = file.Exists
            ? file.Length
            : 0;

        fileWatcher.EnableRaisingEvents = true;
        isRunning = true;
        isPaused = false;
        fileReadThread = new Thread(ReadFileChanges);
        fileReadThread.Start();
    }

    public void Stop()
    {
        if (fileWatcher is null)
        {
            return;
        }

        isRunning = false;
        fileWatcher.EnableRaisingEvents = false;
        fileReadThread?.Join();
    }

    public void Pause()
    {
        isPaused = true;
    }

    public void Resume()
    {
        isPaused = false;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing &&
            fileWatcher is not null)
        {
            fileWatcher.Created -= OnCreatedOrChanged;
            fileWatcher.Changed -= OnCreatedOrChanged;
            fileWatcher.Deleted -= OnDeletedOrRenamed;
            fileWatcher.Renamed -= OnDeletedOrRenamed;
            fileWatcher.Dispose();
        }
    }

    private void OnCreatedOrChanged(
        object sender,
        FileSystemEventArgs e)
    {
        if (!isRunning || isPaused)
        {
            return;
        }

        ReadFileChanges();
    }

    private void OnDeletedOrRenamed(
        object sender,
        FileSystemEventArgs e)
    {
        lock (syncLock)
        {
            lastMaxOffset = 0;
        }
    }

    private void ReadFileChanges()
    {
        while (isRunning && !isPaused)
        {
            try
            {
                if (File.Exists(file.FullName))
                {
                    using var fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    using var sr = new StreamReader(fs);

                    lock (syncLock)
                    {
                        if (fs.Length < lastMaxOffset)
                        {
                            fs.Seek(0, SeekOrigin.End);
                            lastMaxOffset = fs.Position;
                        }
                        else
                        {
                            fs.Seek(lastMaxOffset, SeekOrigin.Begin);
                        }
                    }

                    while (sr.ReadLine() is { } line)
                    {
                        LineAdded?.Invoke(new TailLine(file, line));
                    }

                    lock (syncLock)
                    {
                        lastMaxOffset = fs.Position;
                    }
                }
            }
            catch (IOException)
            {
                // Handle the file being inaccessible
            }

            Thread.Sleep(100); // Prevents tight looping, adjust as needed.
        }
    }
}