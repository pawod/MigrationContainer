using System;

namespace Pawod.MigrationContainer.Filesystem.NTFS
{
    public class Win32Api
    {
        public enum ECreationDisposition : uint
        {
            /// <summary>
            ///     Creates a new file. The function fails if a specified file exists.
            /// </summary>
            NEW = 1,

            /// <summary>
            ///     Creates a new file, always.
            ///     If a file exists, the function overwrites the file, clears the existing
            ///     attributes, combines the specified file attributes,
            ///     and flags with FILE_ATTRIBUTE_ARCHIVE, but does not set the security
            ///     descriptor that the SECURITY_ATTRIBUTES structure specifies.
            /// </summary>
            CREATE_ALWAYS = 2,

            /// <summary>
            ///     Opens a file. The function fails if the file does not exist.
            /// </summary>
            OPEN_EXISTING = 3,

            /// <summary>
            ///     Opens a file, always.
            ///     If a file does not exist, the function creates a file as if
            ///     dwCreationDisposition is CREATE_NEW.
            /// </summary>
            OPEN_ALWAYS = 4,

            /// <summary>
            ///     Opens a file and truncates it so that its size is 0 (zero) bytes. The
            ///     function fails if the file does not exist.
            ///     The calling process must open the file with the GENERIC_WRITE access right.
            /// </summary>
            TRUNCATE_EXISTING = 5
        }

        [Flags]
        public enum EFileAccess : uint
        {
            //
            // Standart Section
            //

            ACCESS_SYSTEM_SECURITY = 0x1000000, // AccessSystemAcl access type
            MAXIMUM_ALLOWED = 0x2000000, // MaximumAllowed access type

            DELETE = 0x10000,
            READ_CONTROL = 0x20000,
            WRITE_DAC = 0x40000,
            WRITE_OWNER = 0x80000,
            SYNCHRONIZE = 0x100000,

            STANDARD_RIGHTS_REQUIRED = 0xF0000,
            STANDARD_RIGHTS_READ = READ_CONTROL,
            STANDARD_RIGHTS_WRITE = READ_CONTROL,
            STANDARD_RIGHTS_EXECUTE = READ_CONTROL,
            STANDARD_RIGHTS_ALL = 0x1F0000,

            FILE_READ_DATA = 0x0001, // file & pipe
            FILE_LIST_DIRECTORY = 0x0001, // directory
            FILE_WRITE_DATA = 0x0002, // file & pipe
            FILE_ADD_FILE = 0x0002, // directory
            FILE_APPEND_DATA = 0x0004, // file
            FILE_ADD_SUBDIRECTORY = 0x0004, // directory
            FILE_CREATE_PIPE_INSTANCE = 0x0004, // named pipe
            FILE_READ_EA = 0x0008, // file & directory
            FILE_WRITE_EA = 0x0010, // file & directory
            FILE_EXECUTE = 0x0020, // file
            FILE_TRAVERSE = 0x0020, // directory
            FILE_DELETE_CHILD = 0x0040, // directory
            FILE_READ_ATTRIBUTES = 0x0080, // all
            FILE_WRITE_ATTRIBUTES = 0x0100, // all

            //
            // Generic Section
            //

            GENERIC_READ = 0x80000000,
            GENERIC_WRITE = 0x40000000,
            GENERIC_EXECUTE = 0x20000000,
            GENERIC_ALL = 0x10000000,

            SPECIFIC_RIGHTS_ALL = 0x00FFFF,
            FILE_ALL_ACCESS = STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE | 0x1FF,

            FILE_GENERIC_READ = STANDARD_RIGHTS_READ | FILE_READ_DATA | FILE_READ_ATTRIBUTES | FILE_READ_EA | SYNCHRONIZE,

            FILE_GENERIC_WRITE = STANDARD_RIGHTS_WRITE | FILE_WRITE_DATA | FILE_WRITE_ATTRIBUTES | FILE_WRITE_EA | FILE_APPEND_DATA | SYNCHRONIZE,

            FILE_GENERIC_EXECUTE = STANDARD_RIGHTS_EXECUTE | FILE_READ_ATTRIBUTES | FILE_EXECUTE | SYNCHRONIZE
        }

        [Flags]
        public enum EFileAttributes : uint
        {
            READONLY = 0x00000001,
            HIDDEN = 0x00000002,
            SYSTEM = 0x00000004,
            DIRECTORY = 0x00000010,
            ARCHIVE = 0x00000020,
            DEVICE = 0x00000040,
            NORMAL = 0x00000080,
            TEMPORARY = 0x00000100,
            SPARSE_FILE = 0x00000200,
            REPARSE_POINT = 0x00000400,
            COMPRESSED = 0x00000800,
            OFFLINE = 0x00001000,
            NOT_CONTENT_INDEXED = 0x00002000,
            ENCRYPTED = 0x00004000,
            WRITE_THROUGH = 0x80000000,
            OVERLAPPED = 0x40000000,
            NO_BUFFERING = 0x20000000,
            RANDOM_ACCESS = 0x10000000,
            SEQUENTIAL_SCAN = 0x08000000,
            DELETE_ON_CLOSE = 0x04000000,
            BACKUP_SEMANTICS = 0x02000000,
            POSIX_SEMANTICS = 0x01000000,
            OPEN_REPARSE_POINT = 0x00200000,
            OPEN_NO_RECALL = 0x00100000,
            FIRST_PIPE_INSTANCE = 0x00080000
        }

        [Flags]
        public enum EFileShare : uint
        {
            /// <summary>
            /// </summary>
            NONE = 0x00000000,

            /// <summary>
            ///     Enables subsequent open operations on an object to request read access.
            ///     Otherwise, other processes cannot open the object if they request read
            ///     access.
            ///     If this flag is not specified, but the object has been opened for read
            ///     access, the function fails.
            /// </summary>
            READ = 0x00000001,

            /// <summary>
            ///     Enables subsequent open operations on an object to request write access.
            ///     Otherwise, other processes cannot open the object if they request write
            ///     access.
            ///     If this flag is not specified, but the object has been opened for write
            ///     access, the function fails.
            /// </summary>
            WRITE = 0x00000002,

            /// <summary>
            ///     Enables subsequent open operations on an object to request delete access.
            ///     Otherwise, other processes cannot open the object if they request delete
            ///     access.
            ///     If this flag is not specified, but the object has been opened for delete
            ///     access, the function fails.
            /// </summary>
            DELETE = 0x00000004
        }

        [Flags]
        public enum OpenFileStyle : uint
        {
            OF_CANCEL = 0x00000800, // Ignored. For a dialog box with a Cancel button, use OF_PROMPT.
            OF_CREATE = 0x00001000, // Creates a new file. If file exists, it is truncated to zero (0) length.
            OF_DELETE = 0x00000200, // Deletes a file.
            OF_EXIST = 0x00004000, // Opens a file and then closes it. Used to test that a file exists
            OF_PARSE = 0x00000100, // Fills the OFSTRUCT structure, but does not do anything else.
            OF_PROMPT = 0x00002000, // Displays a dialog box if a requested file does not exist 
            OF_READ = 0x00000000, // Opens a file for reading only.
            OF_READWRITE = 0x00000002, // Opens a file with read/write permissions.
            OF_REOPEN = 0x00008000, // Opens a file by using information in the reopen buffer.

            // For MS-DOS–based file systems, opens a file with compatibility mode, allows any process on a 
            // specified computer to open the file any number of times.
            // Other efforts to open a file with other sharing modes fail. This flag is mapped to the 
            // FILE_SHARE_READ|FILE_SHARE_WRITE flags of the CreateFile function.
            OF_SHARE_COMPAT = 0x00000000,

            // Opens a file without denying read or write access to other processes.
            // On MS-DOS-based file systems, if the file has been opened in compatibility mode
            // by any other process, the function fails.
            // This flag is mapped to the FILE_SHARE_READ|FILE_SHARE_WRITE flags of the CreateFile function.
            OF_SHARE_DENY_NONE = 0x00000040,

            // Opens a file and denies read access to other processes.
            // On MS-DOS-based file systems, if the file has been opened in compatibility mode,
            // or for read access by any other process, the function fails.
            // This flag is mapped to the FILE_SHARE_WRITE flag of the CreateFile function.
            OF_SHARE_DENY_READ = 0x00000030,

            // Opens a file and denies write access to other processes.
            // On MS-DOS-based file systems, if a file has been opened in compatibility mode,
            // or for write access by any other process, the function fails.
            // This flag is mapped to the FILE_SHARE_READ flag of the CreateFile function.
            OF_SHARE_DENY_WRITE = 0x00000020,

            // Opens a file with exclusive mode, and denies both read/write access to other processes.
            // If a file has been opened in any other mode for read/write access, even by the current process,
            // the function fails.
            OF_SHARE_EXCLUSIVE = 0x00000010,

            // Verifies that the date and time of a file are the same as when it was opened previously.
            // This is useful as an extra check for read-only files.
            OF_VERIFY = 0x00000400,

            // Opens a file for write access only.
            OF_WRITE = 0x00000001
        }
    }
}