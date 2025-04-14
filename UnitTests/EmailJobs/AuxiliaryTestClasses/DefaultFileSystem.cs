using System;
using System.IO;
using App1.Services;
using Moq;
using Xunit;

namespace UnitTests.EmailJobs.AuxiliaryTestClasses
{

    public class DefaultFileSystem : IFileSystem
    {
        public string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }
    }
}