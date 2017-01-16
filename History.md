FUTURE: PerformanceCounters
FUTURE:	Log4Net
FUTURE:	NLog
FUTURE:	CommonLogging
...

CNG:	Перешли на лицензию LGPL V3.0
NEW:	Добавился проект для создания NuGet

V1.1.0.1/1.0.0.9 
File:	ILogWriterAttributes.cs, DiagnosticSettings.cs
NEW:	Поддержка своих атрибутов в конфигурации.

V1.1.0.0/1.0.0.8 
CNG:	Переработан код для поддержки net45
NEW:	Заменен намеспасе на Abc.Diagnostics
NEW:	Поддержка EntrLib:
		V60	Version=6.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
FIX:	Исправлены предупреждения StyleCop

V1.0.0.7
File:	EntrLibLogWriter.cs
FIX:	EntrLib5.0 LogWriterFactory имеет больше одного Create

File:	EntrLib50LogWriter.cs
NEW:	Поддержка EntrLib:
		V50U1	Version=5.0.505.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35

V1.0.0.6
CNG:	LogActivity, TraceUtility, ExceptionUtility тепреь пишут через LogUtility.Write вместо UseDiagnosticTrace(LogUtility).
NEW:	новый статический клас DiagnosticTools
CNG:	LogUtility появился новое свойство LogSourceName
CNG:	Activity пишется в категорию по умолчанию
FIX:	DefaultLogWriter сообщения без категории пишутся в категорию по умолчанию, иначе в категорию Trace.

V1.0.0.5
FIX:	DefaultLogWriter поддреживает IsTraceEnabled
FIX:	Логирование через LogUtility с Exception должно иметь важность Error
NEW:	Поддержка EntrLib:
		V31	Version=3.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
		V40	Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
		V41	Version=4.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
		V50	Version=5.0.414.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35

V1.0.0.4
CNG:	Переведен на VS2010
CNG:	Изменен клас TraceUtility, вместо ILogWriter в конструкторе используется UseDiagnosticTrace(LogUtility)

V1.0.0.3
NEW:	Реализованы сообщения типа Transfer в DefaultLogWriter.cs, LogActivity.cs
CNG:	Добавлены параметры TraceIdentifier, AppDomain в DefaultLogWriter.cs (для удобного просмотра Service Trace Viewer)
FIX:	Подправлена в класе LogUtility.cs функция StackTrace для Exception
CNG:	ILogWriter добавлено свойство и поддержка для IsTracingEnable
CNG:	EntrLib40LogWriter теперь пишет в XML формате.
CNG:	Разделены при записи в лог Exception и Dictionary<> изменеия в методе ILogWriter.Write.

V1.0.0.2
FIX:	LogUtility ArgumentNullException если писать c Exception = null
CNG:	Реализован интерфейс IDisposable в EntrLib40LogWriter.cs
NEW:	добавлен класс для установки DiagnosticInstaller.cs
CNG:	UnmanagedSecurityContextInformationProvider удален, используйте от EnterpriseLibrary
CNG:	DebugInformationProvider и ManagedSecurityContextInformationProvider отсоединены от EnterpriseLibrary
NEW:	IDebugUtility.cs добавлен интерфейс для DebugInformationProvider

CNG:	Переписан класс DiagnosticSettings с использованием ILogWriter.
NEW:	Добавлен класс EntrLib40LogWriter работает c EnterpriseLibrary 4.0
NEW:	Добавлен класс DefaultLogWriter работает по умолчанию с XmlWriterTraceListener
CNG:	Переписан класс TraceUtility с использованием ILogWriter.
NEW:	Добавлен интерфейс ILogWriter.
NEW:	Добавлен DiagnosticSettings класс. Теперь можно использовать разные варианты создания логера.

V1.0.0.1
NEW:	Добавлен LogActivity класс. Небольшие изменения в ExceptionUtility и LogUtility.
CNG:	При записи в EventLog выдавал исключение.
CNG:	Добавлен метод WriteCore класу LogUtility, теперь можно наследовать и переобпределять.
CNG:	Добавлен ExtraIinformatio
