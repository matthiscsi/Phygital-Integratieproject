using System.ComponentModel.DataAnnotations;
using PIP.BL.IManagers;
using PIP.DAL.IRepositories;
using PIP.Domain.Companion;
using PIP.Domain.Flow;

namespace PIP.BL.Managers;

public class FlowManager : IFlowManager
{
    private readonly IFlowRepository _flowRepository;
    private readonly IProjectRepository _projectRepository;

    public FlowManager(IFlowRepository flowRepository, IProjectRepository projectRepository)
    {
        _flowRepository = flowRepository;
        _projectRepository = projectRepository;
    }

    public Flow GetFlowWithProject(long id)
    {
        return _flowRepository.ReadFlowWithProject(id);
    }


    public IEnumerable<Flow> GetFlowsOfProject(long id)
    {
        return _flowRepository.ReadFlowsOfProject(id);
    }

    public Flow GetFlow(long id)
    {
        return _flowRepository.ReadFlow(id);
    }

    public Flow GetFlowWithSteps(long id)
    {
        return _flowRepository.ReadFlowWithSteps(id);
    }

    public Flow AddFlow(long projectId, string title, string description, Theme theme)
    {
        var flow = new Flow
        {
            Project = _projectRepository.ReadProject(projectId),
            Title = title,
            Description = description,
            Theme = theme
        };

        var errors = new List<ValidationResult>();

        var valid = Validator.TryValidateObject(flow, new ValidationContext(flow),
            errors, true);

        if (!valid)
        {
            Console.WriteLine(errors);
            throw new ValidationException("Flow is not valid!");
        }

        _flowRepository.CreateFlow(flow);
        return flow;
    }


    public Flow GetFlowWithThemesAndSubthemes(long flowId)
    {
        return _flowRepository.ReadFlowWithThemesAndSubthemes(flowId);
    }

    public Flow GetFlowWithThemesAndIdeeas(long flowId)
    {
        return _flowRepository.ReadFlowWithThemesAndIdeas(flowId);
    }

    public Flow GetFlowWithSubThemesExpectSelectedSubTheme(Subtheme subtheme)
    {
        return _flowRepository.ReadFlowWithSubThemesExpectSelectedSubTheme(subtheme);
    }

    public IEnumerable<Subtheme> GetSubthemesOfFlow(long flowId)
    {
        return _flowRepository.ReadSubthemesOfFlow(flowId);
    }

    public void UpdateFlow(Flow flow)
    {
        // Retrieve the existing flow from the database
        var existingFlow = _flowRepository.ReadFlow(flow.FlowId);

        if (existingFlow == null)
        {
            throw new Exception("Flow not found");
        }

        // Update the existing flow with the new values
        existingFlow.Title = flow.Title;
        existingFlow.Description = flow.Description;
        existingFlow.Theme = flow.Theme;
        existingFlow.UrlFlowPicture = flow.UrlFlowPicture;

        // Save the changes to the database
        _flowRepository.UpdateFlow(existingFlow);
    }

    public void UpdateFlowSessionCirculaireFlow(long id)
    {
        var flowSession = _flowRepository.ReadFlowSessionWithFlowAndSteps(id);
        foreach (var flow in flowSession.CirculaireFlows!.Flows)
        {
            foreach (var fs in flow.FlowSteps)
            {
                if (fs.OrderNr == 0)
                {
                    flowSession.CurrentFlowStep = fs;
                }
            }
        }

        flowSession.PassedSubthemes = new List<long>();
        flowSession.State = State.Active;
        _flowRepository.UpdateFlowSession(flowSession);
    }

    public Note AddNote(string title, string description, long flowsessionId, long flowstepId)
    {
        var flowsession = _flowRepository.ReadFlowSessionWithFlowAndSteps(flowsessionId);
        var notes = _flowRepository.ReadNotesOfFlowSession(flowsessionId);
        var existingNote = notes.FirstOrDefault(n => n.Title == title);
        if (existingNote != null)
        {
            return _flowRepository.UpdateNote(title,description);
        }
        else
        {
        Note note = new Note()
        {
            Description = description,
            Title = title,
            TimeOfCreation = DateTime.Now,
            FlowSession = flowsession,
            FlowStepId = flowstepId
            
        };
        
            _flowRepository.CreateNote(note);
            return note;
        }
    }

    public Flow GetFlowFromFlowstep(long flowstepId)
    {
        return _flowRepository.ReadFlowFromFlowstep(flowstepId);
    }
}