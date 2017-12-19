using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TheConsultancyFirm.Data;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Controllers
{
    public class NewsletterController : Controller
    {
        private readonly ApplicationDbContext _context;
        public NewsletterController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns the state code for subscribing to the newsletter
        /// </summary>
        /// <param name="newsletter"></param>
        /// <returns>Status code to an ajax call</returns>
        public IActionResult Index(Newsletter newsletter)
        {
            ActionResult state;

            //Validate the input field
            if(ModelState.IsValid && !String.IsNullOrEmpty(newsletter.Email))
            {
                _context.NewsLetters.Add(newsletter);

                if(_context.SaveChanges() == 0)
                {
                    // return 400 code
                    state = BadRequest();
                }
                else
                {
                    //returns a 200 code
                    state = Ok();
                }
            }
            else
            {
                state = BadRequest();
            }
            return state;
        }
    }
}
