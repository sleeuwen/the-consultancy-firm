using System.Collections.Generic;
using System.Linq;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Repositories
{
	public interface ICaseRepository
	{
		Case Get(int id);
		IQueryable<Case> GetAll();
        List<Case> GetSurrounding(Case c);
	}
}