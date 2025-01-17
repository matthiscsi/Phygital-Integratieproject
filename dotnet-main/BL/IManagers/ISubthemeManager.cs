using PIP.Domain.Flow;

namespace PIP.BL.IManagers;

public interface ISubthemeManager
{
    Subtheme AddSubtheme(Subtheme subtheme);
    Subtheme GetSubtheme(long id);
    
    Subtheme DeleteSubtheme(long id);
    Subtheme UpdateSubtheme(Subtheme subtheme);
     
     
}