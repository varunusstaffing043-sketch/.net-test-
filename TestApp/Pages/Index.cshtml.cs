using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TestApp.Models;
using TestApp.Services;

namespace TestApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly CalculatorService _calculator;

        public IndexModel(CalculatorService calculator)
        {
            _calculator = calculator;
        }

        [BindProperty]
        public int FirstNumber { get; set; }

        [BindProperty]
        public int SecondNumber { get; set; }

        public CalculationResult? Result { get; set; }

        public void OnPost()
        {
            Result = _calculator.Add(FirstNumber, SecondNumber);
        }
    }
}
