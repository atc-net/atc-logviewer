# Examples on default log formats

## log4net

```text
2024-02-16 09:15:01,123 [INFO] [Root] - Application Start
2024-02-16 09:15:03,456 [DEBUG] [ModuleLoader] - Loading modules
2024-02-16 09:15:04,789 [INFO] [ModuleA] - Module A initialized
2024-02-16 09:15:05,012 [DEBUG] [ModuleB] - Module B initialization started
2024-02-16 09:15:05,234 [INFO] [ModuleB] - Module B initialized
2024-02-16 09:15:06,345 [WARN] [ModuleC] - Module C is deprecated
2024-02-16 09:15:07,456 [INFO] [ModuleC] - Module C initialized anyway
2024-02-16 09:15:08,567 [DEBUG] [DatabaseConnection] - Attempting connection to database
2024-02-16 09:15:09,678 [ERROR] [DatabaseConnection] - Database connection failed due to timeout
2024-02-16 09:15:10,789 [INFO] [DatabaseRetry] - Retrying database connection
2024-02-16 09:15:11,890 [DEBUG] [DatabaseConnection] - Database connection attempt 2
2024-02-16 09:15:12,901 [INFO] [Database] - Database connected
2024-02-16 09:15:13,012 [WARN] [SystemCheck] - Low disk space
2024-02-16 09:15:14,123 [ERROR] [FileSave] - Failed to save file due to disk quota exceeded
2024-02-16 09:15:15,234 [INFO] [UserAuth] - User login attempt
2024-02-16 09:15:16,345 [INFO] [UserAuth] - User logged in successfully
2024-02-16 09:15:17,456 [DEBUG] [SessionManager] - Session started for user: User123
2024-02-16 09:15:18,567 [WARN] [SessionManager] - User session already active
2024-02-16 09:15:19,678 [INFO] [HealthCheck] - Application health check OK
2024-02-16 09:15:20,789 [INFO] [Root] - Application End
```

## nlog

```text
2024-02-16 09:15:01.123 INFO Application Start
2024-02-16 09:15:03.456 DEBUG Loading modules
2024-02-16 09:15:04.789 INFO Module A initialized
2024-02-16 09:15:05.012 DEBUG Module B initialization started
2024-02-16 09:15:05.234 INFO Module B initialized
2024-02-16 09:15:06.345 WARN Module C is deprecated
2024-02-16 09:15:07.456 INFO Module C initialized anyway
2024-02-16 09:15:08.567 DEBUG Attempting connection to database
2024-02-16 09:15:09.678 ERROR Database connection failed
2024-02-16 09:15:10.789 INFO Retrying database connection
2024-02-16 09:15:11.890 DEBUG Database connection attempt 2
2024-02-16 09:15:12.901 INFO Database connected
2024-02-16 09:15:13.012 WARN Low disk space
2024-02-16 09:15:14.123 ERROR Failed to save file
2024-02-16 09:15:15.234 INFO User login attempt
2024-02-16 09:15:16.345 INFO User logged in successfully
2024-02-16 09:15:17.456 DEBUG Session started for user
2024-02-16 09:15:18.567 WARN User session already active
2024-02-16 09:15:19.678 INFO Application health check OK
2024-02-16 09:15:20.789 INFO Application End
```

## serilog

```text
2024-02-16 09:15:01.123 [INF] Application Start
2024-02-16 09:15:03.456 [DBG] Loading modules
2024-02-16 09:15:04.789 [INF] Module A initialized
2024-02-16 09:15:05.012 [DBG] Module B initialization started
2024-02-16 09:15:05.234 [INF] Module B initialized
2024-02-16 09:15:06.345 [WRN] Module C is deprecated
2024-02-16 09:15:07.456 [INF] Module C initialized anyway
2024-02-16 09:15:08.567 [DBG] Attempting connection to database
2024-02-16 09:15:09.678 [ERR] Database connection failed: "Connection timeout"
2024-02-16 09:15:10.789 [INF] Retrying database connection
2024-02-16 09:15:11.890 [DBG] Database connection attempt 2
2024-02-16 09:15:12.901 [INF] Database connected
2024-02-16 09:15:13.012 [WRN] Low disk space
2024-02-16 09:15:14.123 [ERR] Failed to save file: "Disk quota exceeded"
2024-02-16 09:15:15.234 [INF] User login attempt
2024-02-16 09:15:16.345 [INF] User logged in successfully
2024-02-16 09:15:17.456 [DBG] Session started for user: "User123"
2024-02-16 09:15:18.567 [WRN] User session already active
2024-02-16 09:15:19.678 [INF] Application health check OK
2024-02-16 09:15:20.789 [INF] Application End
```

- [INF] stands for Information.
- [DBG] represents Debug.
- [WRN] is for Warning.
- [ERR] indicates Error.
