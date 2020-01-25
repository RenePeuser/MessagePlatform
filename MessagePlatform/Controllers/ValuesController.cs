using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace MessageApiGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        //// GET api/values/5
        //[HttpGet("{id}")]
        //public ActionResult<string> Get(int id)
        //{
        //    return "value";
        //}

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<CSharpFile> Get(ProtoFile protoFile)
        {
            return new CSharpFile(protoFile.Name.Replace(".proto", ".cs"), "");
        }

        // POST api/values
        [HttpPost]
        public ActionResult Post([FromBody] ProtoFile protoFile)
        {
            var csharpFile = new CSharpFile(protoFile.Name.Replace(".proto", ".cs"), "");
            return Ok(csharpFile);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class LanguagesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<TranspileTargetLanguage>> Get()
        {
            return Ok(GetAll());
        }

        // POST api/values
        [HttpPost]
        public ActionResult<TranspileResult> Post([FromBody] ProtoFile protoFile)
        {

        }

        private IEnumerable<TranspileTargetLanguage> GetAll()
        {
            yield return new TranspileToCSharp();
        }
    }

    public abstract class TranspileTargetLanguage
    {
        protected TranspileTargetLanguage(string languageName, string fileExtension)
        {
            LanguageName = languageName;
            FileExtension = fileExtension;
        }

        public string LanguageName { get; }

        public string FileExtension { get; }
    }

    public class TranspileToCSharp : TranspileTargetLanguage
    {
        public TranspileToCSharp() : base("csharp_out", ".cs")
        {
        }
    }

    public class ProtoFile
    {
        public ProtoFile(string name, string syntax, string fullName)
        {
            Name = name;
            Syntax = syntax;
            FullName = fullName;
        }

        public string FullName { get; }
        public string Name { get; }
        public string Syntax { get; }
    }

    public class TranspileResult
    {
        public ProtoFile ProtoFile { get; }
        public TranspileTargetLanguage Language { get; }
        public string SyntaxtTree { get; }

        public TranspileResult(ProtoFile protoFile, string syntaxtTree, TranspileTargetLanguage language)
        {
            ProtoFile = protoFile;
            SyntaxtTree = syntaxtTree;
            Language = language;
        }
    }

    public class CSharpFile
    {
        public CSharpFile(string name, string syntaxtree)
        {
            Name = name;
            Syntaxtree = syntaxtree;
        }

        public string Name { get; set; }
        public string Syntaxtree { get; set; }
    }
}
