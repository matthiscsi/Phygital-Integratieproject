using PIP.Domain.Flow;

namespace PIP.DAL.IRepositories;

public interface ISubthemeRepository
{
    Subtheme CreateSubtheme(Subtheme subtheme);
    Subtheme ReadSubtheme(long id);
      
    Subtheme DeleteSubtheme(long id);
    Subtheme UpdateSubtheme(Subtheme subtheme);
    



}