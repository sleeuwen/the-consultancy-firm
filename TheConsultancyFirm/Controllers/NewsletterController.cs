using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TheConsultancyFirm.Data;
using TheConsultancyFirm.Models;
using TheConsultancyFirm.Repositories;

namespace TheConsultancyFirm.Controllers
{
    public class NewsletterController : Controller
    {
        private readonly INewsletterRepository _repository;
        public NewsletterController(INewsletterRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Returns the state code for subscribing to the newsletter
        /// </summary>
        /// <param name="newsletter"></param>
        /// <returns>Status code to an ajax call</returns>
        [HttpPost]
        public IActionResult Index(Newsletter newsletter)
        {
            ActionResult state;

            //Validate the input field
            if(ModelState.IsValid && !String.IsNullOrEmpty(newsletter.Email))
            {
                if(_repository.Subscribe(newsletter) == 0)
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
