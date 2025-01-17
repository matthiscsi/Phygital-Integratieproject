using Microsoft.EntityFrameworkCore;
using PIP.DAL.IRepositories;
using PIP.Domain.Companion;
using Pip.Domain.Flow;
using PIP.Domain.Flow;

namespace PIP.DAL.EF.Repositories;

public class FlowRepository : IFlowRepository
{
    private PhygitalDbContext _context;

    public FlowRepository(PhygitalDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Flow> ReadAllFlows()
    {
        return _context.Flows;
    }

    public void UpdateFlow(Flow flow)
    {
        _context.Flows.Update(flow);
        _context.SaveChanges();
    }

    public Flow ReadFlow(long flowId)
    {
        //TODO: Add Include for FlowSteps
        return _context.Flows.Include(f => f.Project).Include(f => f.Theme)
                .SingleOrDefault(f => f.FlowId == flowId)
            ;
    }

    public Flow ReadFlowWithSteps(long flowId)
    {
        var flow = _context.Flows.Where(f => f.FlowId == flowId).Include(f => f.FlowSteps.OrderBy(fs=>fs.OrderNr)).SingleOrDefault();
        return flow;
    }

    public IEnumerable<Flow> ReadFlowsOfProject(long id)
    {
        return _context.Flows.Include(f => f.Project)
            .ThenInclude(p => p.Subplatform)
            .Include(f => f.FlowSteps).Where(f => f.Project.ProjectId == id);
    }

    public Flow ReadFlowWithProject(long id)
    {
        return _context.Flows
            .Include(f => f.Project).ThenInclude(p=>p.SubPlatformAdmin)
            .Include(f => f.Theme)
            .Include(f => f.FlowSteps)
            .Where(f => f.FlowId == id)
            .SingleOrDefault();
    }


    public FlowSession CreateFlowSession(FlowSession flowSession)
    {
        int ordernr = 0;
        List<FlowStep> rankedFlowsteps = new List<FlowStep>();
        foreach (var fs in flowSession.Flow.FlowSteps)
        {
            fs.OrderNr = ordernr;
            rankedFlowsteps.Add(fs);
            ordernr++;
        }

        flowSession.Flow.FlowSteps = rankedFlowsteps;
        flowSession.CurrentFlowStep = flowSession.Flow.FlowSteps.FirstOrDefault(f => f.OrderNr == 0);
        _context.FlowSessions.Add(flowSession);
        _context.SaveChanges();
        return flowSession;
    }

    public FlowSession CreateCirculaireFlowSession(FlowSession flowSession)
    {
        List<FlowStep> flowSteps = new List<FlowStep>();
        int orderNr = 0;
        foreach (var flow in flowSession.CirculaireFlows!.Flows)
        {
            //misschien nog randomizen
            foreach (var flowstep in flow.FlowSteps)
            {
                flowstep.OrderNr = orderNr;
                orderNr++;
                flowSteps.Add(flowstep);
            }
        }

        flowSession.CurrentFlowStep = flowSteps.FirstOrDefault(f => f.OrderNr == 0);
        _context.FlowSessions.Add(flowSession);
        _context.SaveChanges();
        return flowSession;
    }


    public FlowSession ReadFlowSessionWithFlowAndSteps(long flowSessionId)
    {
        return _context.FlowSessions
            .Where(fs => fs.FlowSessionId == flowSessionId)
            .Include(fs => fs.CurrentFlowStep)
            .Include(fs => fs.CirculaireFlows)
                .ThenInclude(cf => cf.Flows)
                .ThenInclude(fs => fs.FlowSteps)
                .ThenInclude(fs => fs.SubTheme)
            .Include(fs => fs.CirculaireFlows)
            .ThenInclude(cf => cf.Flows)
            .ThenInclude(f => f.Project)
            .ThenInclude(p=>p.Subplatform)
            .Include(fs => fs.Flow)
                .ThenInclude(f => f.FlowSteps.OrderBy(step => step.OrderNr))
                .ThenInclude(f => f.SubTheme)
            .Include(f => f.Flow)
                .ThenInclude(f => f.Project)
                .ThenInclude(p => p.Subplatform)
            .SingleOrDefault();
    }


    public void UpdateFlowSession(FlowSession flowSession)
    {
        _context.FlowSessions.Update(flowSession);
        _context.SaveChanges();
    }

    public void SkipSubtheme(long subthemeId, long flowSessionId)
    {
        var flowSession = _context.FlowSessions
            .SingleOrDefault(fs => fs.FlowSessionId == flowSessionId);

        var subtheme = _context.Subthemes.SingleOrDefault(s => s.SubthemeId == subthemeId);

        if (flowSession != null && subtheme != null)
        {
            flowSession.PassedSubthemes.Add(subthemeId);
        }

        UpdateFlowSession(flowSession);
    }

    public Flow CreateFlow(Flow flow)
    {
        _context.Flows.Add(flow);
        _context.SaveChanges();
        return flow;
    }

    public Flow ReadFlowWithThemesAndIdeas(long flowId)
    {
     return   _context.Flows
         .Include(f => f.Theme)
         .ThenInclude(t => t.Ideas).Include(p=>p.Project)
         .SingleOrDefault(f => f.FlowId == flowId);
    }

    public Flow ReadFlowWithThemesAndSubthemes(long flowId)
    {
        return _context.Flows
            .Include(f => f.Theme)
            .ThenInclude(t => t.SubThemes).Include(f=>f.Project).ThenInclude(p=>p.SubPlatformAdmin)
            .SingleOrDefault(f => f.FlowId == flowId);
    }

    public Flow ReadFlowWithSubThemesExpectSelectedSubTheme(Subtheme subtheme)
    {
        var flow = _context.Flows.Include(p=>p.Project).ThenInclude(p=>p.SubPlatformAdmin)
            .Include(f => f.Theme)
            .ThenInclude(t => t.SubThemes)
            .FirstOrDefault(f => f.Theme.SubThemes.Any(st => st.SubthemeId == subtheme.SubthemeId));

        if (flow != null)
        {
            flow.Theme.SubThemes = flow.Theme.SubThemes.Where(st => st.SubthemeId != subtheme.SubthemeId).ToList();
        }

        return flow;
    }

    public int ReadFlowSessionCount(long flowId)
    {
        return _context.FlowSessions.Where(fs => fs.Flow.FlowId == flowId).Count();
    }

    public int ReadSubthemeSkippedCountForFlow(long flowId, long subthemeId)
    {
        return _context.FlowSessions
            .Where(fs => fs.Flow.FlowId == flowId)
            .AsEnumerable()
            .Where(fs => fs.PassedSubthemes.Contains(subthemeId))
            .Count();
    }

    public IEnumerable<Subtheme> ReadSubthemesOfFlow(long flowId)
    {
        var flow = ReadFlowWithThemesAndSubthemes(flowId);
        return flow.Theme.SubThemes;
    }

    public int ReadAverageTimeSpentForFlow(long flowId)
    {
        var flowSessions = _context.FlowSessions.Where(fs => fs.Flow.FlowId == flowId);

        if (flowSessions.Any())
        {
            int averageTimeSpent = (int)flowSessions.Average(fs => fs.ElapsedTime);
            return averageTimeSpent;
        }
        else
        {
            return 0;
        }
    }

    public void CreateNote(Note note)
    {
    
        
        _context.Notes.Add(note);
        _context.SaveChanges();

        
    }

    

    public IEnumerable<Note> ReadNotesOfFlowSession(long id)
    {
        var flowSessionWithNotes = _context.FlowSessions
            .Where(f => f.FlowSessionId == id)
            .SelectMany(fs => fs.Notes)
            .ToList();

        return flowSessionWithNotes;
    }

    public Note UpdateNote(string title, string description)
    {
        var note = _context.Notes.FirstOrDefault(n => n.Title == title);
        note!.Description = description;
        _context.Notes.Update(note);
        _context.SaveChanges();
        return note;
    }

    public Flow ReadFlowFromFlowstep(long flowstepId)
    {
        return _context.FlowSteps
            .Include(fs => fs.Flow)
 
            .FirstOrDefault(fs => fs.FlowStepId == flowstepId).Flow;
    }
}