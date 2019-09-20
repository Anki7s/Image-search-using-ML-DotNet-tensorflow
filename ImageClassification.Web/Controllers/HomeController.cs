using ImageClassification.ImageDataStructures;
using ImageClassification.ModelScorer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace ImageClassification.Web.Controllers
{
    public class HomeController : Controller
    {
        //Index Action Method [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        //Index Action Method [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Index(IFormFile image)
        {
            string assetsRelativePath = @"../../../assets";
            string assetsPath = GetAbsolutePath(assetsRelativePath);
            var tagsTsv = Path.Combine(assetsPath, "inputs", "images", "tags2.tsv");
            var imagesFolder = Path.Combine(assetsPath, "inputs", "images");
            var inceptionPb = Path.Combine(assetsPath, "inputs", "inception", "tensorflow_inception_graph.pb");
            var labelsTxt = Path.Combine(assetsPath, "inputs", "inception", "imagenet_comp_graph_label_strings.txt");

            using (var ms = new FileStream("D:\\MACHINE LEARNING\\ImageClassificationusingML.NET\\Image Classification - Copy\\ImageClassification.Web\\wwwroot\\img\\img.jpg", FileMode.Create))
            {
                await image.CopyToAsync(ms);
            }
            try
            {
                var filePath = Path.GetTempFileName();
                using (var ms = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(ms);
                }
                var modelScorer = new TFModelScorer(tagsTsv, imagesFolder, inceptionPb, labelsTxt);

                var prediction = (ImageNetDataProbability)modelScorer.Score(new ImageNetData() { ImagePath = filePath, Label = "" });
                ViewBag.PredictedLabel = prediction.PredictedLabel;
                ViewBag.Probability = prediction.Probability;
            }
            catch
            {
                return View();
            }
            return View();
        }

        //GetAbsolutePath Action Method
        public static string GetAbsolutePath(string relativePath)
        {
            FileInfo _dataRoot = new FileInfo(typeof(Program).Assembly.Location);
            string assemblyFolderPath = _dataRoot.Directory.FullName;
            string fullPath = Path.Combine(assemblyFolderPath, relativePath);
            return fullPath;
        }
    }
}
