using Microsoft.AspNetCore.Identity;
using PIP.Domain.Deelplatform;
using Pip.Domain.Flow;
using PIP.Domain.Flow;
using PIP.Domain.Flow.Inquiry;
using PIP.Domain.User;
using PIP.Domain.WebApplication;


namespace PIP.DAL.EF;

// public DbSet<Theme.Theme> Themes { get; set; }
// public DbSet<Participant> Participants { get; set; }
// public DbSet<Response> Responses { get; set; }

public class DataSeeder
{
    public async static Task Seed(PhygitalDbContext context, UserManager<IdentityUser> userManager)
    {
        #region Ideeen

        var idea1 = new Idea
        {
            Title = "Mijn idee! Mentale gezondheid op school",
            Description = "Meer aandacht voor mentale gezondheid op school is belangrijk voor het welzijn van jongeren."
        };
        var idea2 = new Idea
        {
            Title = "Een idee: Gezonde voeding op school",
            Description =
                "Het promoten van gezonde voeding op school kan bijdragen aan een gezondere levensstijl voor jongeren."
        };

        var ideasList = new List<Idea>();
        ideasList.Add(idea1);
        ideasList.Add(idea2);
        context.AddRange(idea1, idea2);

        #endregion

        //Platforms

        #region Platforms

        var subplatform1 = new Subplatform
        {
            CustomerName = "StadMechelen",
            Email = "admin@stadmechelen.be",
            MainText =
                "Welkom bij het participatieplatform van Stad Mechelen. Hier kunt u deelnemen aan verschillende projecten en initiatieven die de stad aangaan.",
            Link = "https://www.google.com"
        };
        var subplatform2 = new Subplatform
        {
            CustomerName = "VZW Jeugdwerking",
            Email = "info@jeugdwerking.be",
            MainText =
                "Welkom bij het participatieplatform van VZW Jeugdwerking. Hier kunt u deelnemen aan verschillende projecten en initiatieven die de jeugd aangaan.",
            Link = "https://www.google.com"
        };

        context.Subplatforms.AddRange(subplatform1, subplatform2);

        #endregion

        var subplatformAdmin = await userManager.FindByEmailAsync("Sadmin@kdg.be") as SubPlatformAdministrator;
        var companion = await userManager.FindByEmailAsync("companion@kdg.be") as Companion;

        if (subplatformAdmin != null)
        {
            subplatformAdmin.Subplatform = subplatform1;
            await userManager.UpdateAsync(subplatformAdmin);
        }

        if (companion != null)
        {
            companion.Subplatform = subplatform1;
            await userManager.UpdateAsync(companion);
        }
        //Projects

        #region Project

        var involveTheYouth = new Project
        {
            CirculaireFlow = false,
            Description =
                "Elke stem telt en elke gedachte heeft kracht. Deel jouw unieke perspectief en help ons de toekomst vorm te geven. Jouw ideeën zijn de sleutel tot verandering. Jouw stem, jouw kracht!\n\n",
            IsActive = true,
            Name = "InvolveTheYouth",
            Subplatform = subplatform1,
            SubPlatformAdmin = subplatformAdmin
        };

        var jeugdOpVoorgrond = new Project
        {
            Description =
                "Een programma gericht op het prioriteren van jongerenkwesties en het geven van een stem aan de jeugd in maatschappelijke beslissingen.",
            IsActive = false,
            Name = "Jeugd op de Voorgrond",
            Subplatform = subplatform1,
            SubPlatformAdmin = subplatformAdmin,
            CirculaireFlow = false
        };

        context.Projects.AddRange(
            involveTheYouth,
            jeugdOpVoorgrond
        );

        #endregion

        //Themes

        #region Themes

        //subthemas
        var gezondEten = new Subtheme
        {
            Title = "gezond eten",
            Body = "Het bevorderen van een gezonde levensstijl en het bieden van ondersteuning voor welzijnskwesties"
        };
        var mentaal = new Subtheme
        {
            Title = "mentale gezondheid ontwikkeling",
            Body = "Het bevorderen van een gezonde levensstijl en het bieden van ondersteuning voor welzijnskwesties"
        };

        var energie = new Subtheme
        {
            Title = "Hernieuwbare energie",
            Body = "Het bevorderen van het gebruik van hernieuwbare energiebronnen voor een duurzame toekomst"
        };
        var openbaarVervoer = new Subtheme
        {
            Title = "Openbaar Vervoer",
            Body = "Het bevorderen van het gebruik van hernieuwbare energiebronnen voor een duurzame toekomst"
        };
        var subthemes = new List<Subtheme>();

        subthemes.Add(gezondEten);
        subthemes.Add(mentaal);


        var subthemes2 = new List<Subtheme>();
        subthemes2.Add(openbaarVervoer);
        subthemes2.Add(energie);


        var jongerenHulp = new Theme
        {
            Title = "Jongeren hulp",
            Body = "Hulp bieden aan jongeren via verschillende manieren en zeker ook het mentaal welzijn",
            SubThemes = subthemes,
            UrlThemePicture = "https://storage.googleapis.com/phygitalmediabucket/Youthaid.jpg.jpg",
            Ideas = ideasList
        };
        Theme duurzaamheid = new Theme()
        {
            Title = "Duurzaamheid",
            Body = "Het bevorderen van milieubewustzijn en het nemen van maatregelen voor een duurzame toekomst",
            SubThemes = subthemes2,
            UrlThemePicture = "https://storage.googleapis.com/phygitalmediabucket/Sustainability.jpg",
            Ideas = ideasList
        };

        context.Themes.AddRange(
            jongerenHulp, duurzaamheid
        );

        #endregion

        //Questions

        #region Questions

        var options1Mentaal = new List<Option>();
        options1Mentaal.Add(new Option
            { Text = "Invoering van verplichte lessen over mentale gezondheid en welzijn in het curriculum" });
        options1Mentaal.Add(new Option
        {
            Text = "Toename van de beschikbaarheid van schoolpsychologen en counselors voor studentenondersteuning"
        });
        options1Mentaal.Add(new Option
        {
            Text =
                "Training van leerkrachten om mentale gezondheidsproblemen bij studenten te herkennen en aan te pakken"
        });
        options1Mentaal.Add(new Option
        {
            Text =
                "Creëren van peer support-programma's waarbij studenten elkaar kunnen ondersteunen bij mentale gezondheidskwesties"
        });

        // Closed question 1
        Question question1Mentaal = new ClosedQuestion
        {
            Query = "Wat vindt u van de huidige benadering van mentale gezondheidseducatie in het onderwijs?",
            SubTheme = mentaal,
            Options = options1Mentaal,
            IsActive = true
        };


        foreach (var option in options1Mentaal) option.Question = question1Mentaal;

        var options2 = new List<Option>();
        options2.Add(new Option
            { Text = "Implementatie van verplichte voorlichtingsprogramma's over geestelijke gezondheid" });
        options2.Add(new Option
            { Text = "Aanstellen van schoolpsychologen of counselors voor toegankelijke ondersteuning" });
        options2.Add(new Option
        {
            Text = "Opzetten van peer support-programma's waarbij oudere leerlingen jongere leerlingen ondersteunen"
        });
        options2.Add(new Option
        {
            Text =
                "Organiseren van regelmatige workshops en trainingen voor leerkrachten over het herkennen en omgaan met mentale gezondheidsproblemen bij studenten"
        });
        // Closed question 2 JongerenHulp
        Question question2Mentaal = new ClosedQuestion
        {
            Query = "Hoe zouden wij mentale hulp beter kunnen aankaarten op scholen?",
            SubTheme = mentaal,
            Options = options2,
            IsActive = true
        };


        foreach (var option in options2) option.Question = question2Mentaal;

        var options3 = new List<Option>();
        options3.Add(new Option
            { Text = "Meer investeren in hernieuwbare energiebronnen zoals zonne- en windenergie" });
        options3.Add(new Option
        {
            Text =
                "Implementatie van strengere regelgeving voor de industrie om de uitstoot van broeikasgassen te verminderen"
        });
        options3.Add(
            new Option { Text = "Bevordering van duurzame vervoerswijzen zoals elektrische auto's en fietsen" });
        options3.Add(new Option
        {
            Text =
                "Educatie en bewustwording bevorderen over klimaatverandering en duurzaamheid in het onderwijs en de samenleving"
        });
        // Multiple choice question DUURZAAMHEID
        Question question3Energie = new MultipleChoiceQuestion
        {
            Query =
                "Wat is uw mening over de klimaatverandering en welke maatregelen zouden volgens u genomen moeten worden?",
            SubTheme = energie,
            Options = options3,
            IsActive = true
        };
        context.Questions.Add(question2Mentaal);


        foreach (var option in options3) option.Question = question3Energie;


        //duurzaamheid
        Question question4Energie = new OpenQuestion
        {
            Query = "Welke maatregelen neem jij om je co2 te verminderen?", SubTheme = energie,
            IsActive = true
        };
        Question question5GezondEten = new OpenQuestion
        {
            Query = "Wordt er veel gezond eten gepromoot op reclame? zoniet of zowel kun je enkele voorbeelden geven?",
            SubTheme = gezondEten,
            IsActive = true
        };
        var options6 = new List<Option>();
        options6.Add(new Option { Text = "Betrouwbaar en efficiënt." });
        options6.Add(
            new Option { Text = "Het moet verbeterd worden met betere frequentie en dekking." });
        options6.Add(new Option
            { Text = "Te duur in vergelijking met andere vervoerswijzen." });
        options6.Add(new Option
        {
            Text =
                "Het heeft meer milieuvriendelijke opties nodig, zoals elektrische bussen en treinen."
        });

        Question question6OpenbaarVervoer = new MultipleChoiceQuestion
        {
            Query = "Wat vindt u van het huidige openbaar vervoer?",
            SubTheme = openbaarVervoer,
            Options = options6,
            IsActive = true
        };
        Question question7 = new RangeQuestion
        {
            SubTheme = openbaarVervoer,
            MaxValue = 10,
            MinValue = 1,
            Query = "Hoe tevreden bent u over het openbaar vervoer in uw stad?",
            IsActive = true
        };
        Question question8 = new RangeQuestion
        {
            SubTheme = mentaal,
            MaxValue = 60,
            MinValue = 18,
            Query = "Hoe oud ben je?",
            IsActive = true
        };


        foreach (var option in options6) option.Question = question6OpenbaarVervoer;

        context.Questions.AddRange(question1Mentaal, question3Energie, question4Energie,
            question6OpenbaarVervoer, question7, question8);

        #endregion

        //Answers

        #region Answers

        var closedAnswer1Mentaal1 = new ClosedAnswer { SelectedAnswer = 1 }; // Optie 1 is geselecteerd
        var closedAnswer1Mentaal2 = new ClosedAnswer { SelectedAnswer = 2 }; // Optie 2 is geselecteerd
        var closedAnswer1Mentaal3 = new ClosedAnswer { SelectedAnswer = 3 }; // Optie 3 is geselecteerd

        var closedAnswer2Mentaal1 = new ClosedAnswer { SelectedAnswer = 1 }; // Optie 1 is geselecteerd
        var closedAnswer2Mentaal2 = new ClosedAnswer { SelectedAnswer = 2 }; // Optie 2 is geselecteerd
        var closedAnswer2Mentaal3 = new ClosedAnswer { SelectedAnswer = 3 }; // Optie 3 is geselecteerd

        var multipleChoiceAnswer3Energie1 =
            new MultipleChoiceAnswer { SelectedAnswers = new List<long> { 1, 2 } }; // Optie 1 en 2 zijn geselecteerd
        var multipleChoiceAnswer3Energie2 =
            new MultipleChoiceAnswer { SelectedAnswers = new List<long> { 2, 3 } }; // Optie 2 en 3 zijn geselecteerd
        var multipleChoiceAnswer3Energie3 =
            new MultipleChoiceAnswer { SelectedAnswers = new List<long> { 1, 3 } }; // Optie 1 en 3 zijn geselecteerd

        var openAnswer4Energie = new OpenAnswer
        {
            Answer =
                "Ik verminder mijn CO2-uitstoot door minder te rijden en meer gebruik te maken van openbaar vervoer."
        };

        var openAnswer5GezondEten = new OpenAnswer
        {
            Answer =
                "Er wordt niet genoeg gezond eten gepromoot in reclames. Vaak zijn ongezonde voedingsmiddelen prominent aanwezig in reclames."
        };

        var multipleChoiceAnswer6OpenbaarVervoer1 =
            new MultipleChoiceAnswer { SelectedAnswers = new List<long> { 1 } }; // Optie 1 is geselecteerd
        var multipleChoiceAnswer6OpenbaarVervoer2 =
            new MultipleChoiceAnswer { SelectedAnswers = new List<long> { 2 } }; // Optie 2 is geselecteerd
        var multipleChoiceAnswer6OpenbaarVervoer3 =
            new MultipleChoiceAnswer { SelectedAnswers = new List<long> { 3 } }; // Optie 3 is geselecteerd


        ClosedAnswer conditionalPointAnswer = new ClosedAnswer()
        {
            SelectedAnswer = 1,
            ConditionalPoint = null
        };


        ClosedQuestion followUpStep = new ClosedQuestion
        {
            Query = "Wat vindt u van de huidige benadering van mentale gezondheidseducatie in het onderwijs lmao xd?",
            SubTheme = mentaal,
            IsActive = true,
            ConditionalPoint = null
        };
        var newOptions = new List<Option>
        {
            new Option()
            {
                Question = followUpStep,
                Text = "wa doede gij a dinge"
            },
            new Option()
            {
                Question = followUpStep,
                Text = "EEEEEEEEEEE STOP"
            },
            new Option
            {
                Question = followUpStep,
                Text = "nesh kweeni a dinge"
            }
        };
        followUpStep.Options = newOptions;
        ConditionalPoint conditionalPointQuestion2Mentaal = new ConditionalPoint()
        {
            Question = question2Mentaal,
            Criteria = conditionalPointAnswer,
            FollowUpStep = followUpStep
        };
        context.ConditionalPoints.Add(conditionalPointQuestion2Mentaal);
        conditionalPointAnswer.ConditionalPoint = conditionalPointQuestion2Mentaal;
        followUpStep.ConditionalPoint = conditionalPointQuestion2Mentaal;
        question2Mentaal.QuestionConditionalPoints.Add(conditionalPointQuestion2Mentaal);
        context.FlowSteps.Add(followUpStep);
        context.AddRange(closedAnswer1Mentaal1, closedAnswer1Mentaal2, closedAnswer1Mentaal3,
            closedAnswer2Mentaal1, closedAnswer2Mentaal2, closedAnswer2Mentaal3,
            multipleChoiceAnswer3Energie1, multipleChoiceAnswer3Energie2, multipleChoiceAnswer3Energie3,
            openAnswer4Energie,
            openAnswer5GezondEten,
            multipleChoiceAnswer6OpenbaarVervoer1, multipleChoiceAnswer6OpenbaarVervoer2,
            multipleChoiceAnswer6OpenbaarVervoer3);
        context.Answers.Add(conditionalPointAnswer);

        #endregion



        #region Info
        
        var energieTekst = new Info
        {
            Body =
                "Duurzaamheid is cruciaal. Door hernieuwbare energie en efficiëntie te omarmen, kunnen we onze planeet beter beschermen.",
            SubTheme = energie,
            IsActive = true
        };

        var mentaalTekst = new Info
        {
            Body =
                "Mentale gezondheid voor jongeren is belangrijk. Toegankelijke hulp zoals counseling kan hen helpen met stress en uitdagingen.",
            SubTheme = mentaal,
            IsActive = true
        };

        var openbaarVervoerTekst = new Info
        {
            Body =
                "Goed openbaar vervoer is essentieel voor een groene, toegankelijke stad. Investeren hierin bevordert een duurzame samenleving.",
            SubTheme = openbaarVervoer,
            IsActive = true
        };

        var gezondEtenTekst = new Info
        {
            Body =
                "Gezond eten is belangrijk voor welzijn. Toegang tot verse voeding en bewustwording helpt mensen betere keuzes te maken.",
            SubTheme = gezondEten,
            IsActive = true
        };



        context.Infos.AddRange(mentaalTekst, openbaarVervoerTekst, gezondEtenTekst, energieTekst); //, info3, info4);

        #endregion




        #region Flowsteps

        var flowStepsforJongerenHulp = new List<FlowStep>();
        flowStepsforJongerenHulp.Add(mentaalTekst);
        flowStepsforJongerenHulp.Add(question1Mentaal);
        flowStepsforJongerenHulp.Add(question2Mentaal);
        flowStepsforJongerenHulp.Add(followUpStep);
        flowStepsforJongerenHulp.Add(gezondEtenTekst);
        flowStepsforJongerenHulp.Add(question5GezondEten);
        flowStepsforJongerenHulp.Add(question8);

        int order = 0;
        foreach (var step in flowStepsforJongerenHulp)
        {
            step.OrderNr = order;
            order++;
        }

        order = 0;

        var flowStepsforDuuzaamheid = new List<FlowStep>();
        flowStepsforDuuzaamheid.Add(energieTekst);
        flowStepsforDuuzaamheid.Add(question3Energie);
        flowStepsforDuuzaamheid.Add(question4Energie);
        flowStepsforDuuzaamheid.Add(openbaarVervoerTekst);
        flowStepsforDuuzaamheid.Add(question6OpenbaarVervoer);
        flowStepsforDuuzaamheid.Add(question7);
        foreach (var step in flowStepsforDuuzaamheid)
        {
            step.OrderNr = order;
            order++;
        }

        #endregion

        //Flows

        #region Flow

        // Create a Flow for each project
        var jongerenHulpFlow = new Flow
        {
            Title = "JongerenHulp",
            Description = "Nieuw programma ter ondersteuning van jongeren, met counseling, educatie over mentale gezondheid en peer support-groepen.",
            Project = involveTheYouth,
            FlowSteps = flowStepsforJongerenHulp,
            Theme = jongerenHulp,
            UrlFlowPicture = "https://storage.googleapis.com/phygitalmediabucket/Youthaid.jpg"
        };

        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
        {
            jongerenHulpFlow.Physical = true;
        }

        var duurzaamheidFlow = new Flow
        {
            Title = "Duurzaamheid",
            Description =
                "Een initiatief voor duurzaamheid met educatieve en praktische acties voor een groenere toekomst.",
            Project = involveTheYouth,
            FlowSteps = flowStepsforDuuzaamheid,
            Theme = duurzaamheid,
            UrlFlowPicture = "https://storage.googleapis.com/phygitalmediabucket/Sustainability.jpg",
        };
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
        {
            duurzaamheidFlow.Physical = true;
        }


        context.Flows.AddRange(
            duurzaamheidFlow, jongerenHulpFlow
        );

        #endregion

        #region Sessions

        //Sessions
        var session1 = new FlowSession
        {
            Flow = duurzaamheidFlow,
            State = State.Done,
            PassedSubthemes = new List<long>()
            {
                3
            },
            SessionStartDate = DateTime.Now - TimeSpan.FromMinutes(2.7),
            SessionEndDate = DateTime.Now
        };
        TimeSpan timeDifference1 = session1.SessionEndDate - session1.SessionStartDate;
        double secondsDifference1 = timeDifference1.TotalSeconds;
        session1.ElapsedTime = (int)secondsDifference1;

        var session2 = new FlowSession
        {
            Flow = duurzaamheidFlow,
            State = State.Done,
            PassedSubthemes = new List<long>()
            {
                3, 4
            },
            SessionStartDate = DateTime.Now - TimeSpan.FromMinutes(1.2),
            SessionEndDate = DateTime.Now,
        };
        TimeSpan timeDifference2 = session2.SessionEndDate - session2.SessionStartDate;
        double secondsDifference2 = timeDifference2.TotalSeconds;
        session2.ElapsedTime = (int)secondsDifference2;

        var session3 = new FlowSession
        {
            Flow = duurzaamheidFlow,
            State = State.Done,
            PassedSubthemes = new List<long>()
            {
                3
            },
            SessionStartDate = DateTime.Now - TimeSpan.FromMinutes(0.8),
            SessionEndDate = DateTime.Now
        };
        TimeSpan timeDifference3 = session3.SessionEndDate - session3.SessionStartDate;
        double secondsDifference3 = timeDifference3.TotalSeconds;
        session3.ElapsedTime = (int)secondsDifference3;

        var session4 = new FlowSession
        {
            Flow = duurzaamheidFlow,
            State = State.Done,
            PassedSubthemes = new List<long>()
            {
                3
            },
            SessionStartDate = DateTime.Now - TimeSpan.FromMinutes(2.3),
            SessionEndDate = DateTime.Now
        };

        TimeSpan timeDifference4 = session4.SessionEndDate - session4.SessionStartDate;
        double secondsDifference4 = timeDifference4.TotalSeconds;
        session4.ElapsedTime = (int)secondsDifference4;

        var session5 = new FlowSession
        {
            Flow = duurzaamheidFlow,
            State = State.Done,
            PassedSubthemes = new List<long>()
            {
                4
            },
            SessionStartDate = DateTime.Now - TimeSpan.FromMinutes(8),
            SessionEndDate = DateTime.Now - TimeSpan.FromMinutes(5)
        };

        TimeSpan timeDifference5 = session5.SessionEndDate - session5.SessionStartDate;
        double secondsDifference5 = timeDifference5.TotalSeconds;
        session4.ElapsedTime = (int)secondsDifference5;


        context.FlowSessions.Add(session1);
        context.FlowSessions.Add(session2);
        context.FlowSessions.Add(session3);
        context.FlowSessions.Add(session4);
        context.FlowSessions.Add(session4);

        #endregion

        //Responses

        #region Responses

        var responseConditionalPoint = new Response
        {
            Answer = conditionalPointAnswer,
            Question = question2Mentaal, FlowSession = session1
        };
        var response1Mentaal1 = new Response
            { Answer = closedAnswer1Mentaal1, Question = question1Mentaal, FlowSession = session1 };
        closedAnswer1Mentaal1.Response = response1Mentaal1;

        var response1Mentaal2 = new Response
            { Answer = closedAnswer1Mentaal2, Question = question1Mentaal, FlowSession = session1 };
        closedAnswer1Mentaal2.Response = response1Mentaal2;

        var response1Mentaal3 = new Response
            { Answer = closedAnswer1Mentaal3, Question = question1Mentaal, FlowSession = session1 };
        closedAnswer1Mentaal3.Response = response1Mentaal3;

        var response2Mentaal1 = new Response
            { Answer = closedAnswer2Mentaal1, Question = question2Mentaal, FlowSession = session1 };
        closedAnswer2Mentaal1.Response = response2Mentaal1;

        var response2Mentaal2 = new Response
            { Answer = closedAnswer2Mentaal2, Question = question2Mentaal, FlowSession = session1 };
        closedAnswer2Mentaal2.Response = response2Mentaal2;

        var response2Mentaal3 = new Response
            { Answer = closedAnswer2Mentaal3, Question = question2Mentaal, FlowSession = session1 };
        closedAnswer2Mentaal3.Response = response2Mentaal3;

        var response3Energie1 = new Response
            { Answer = multipleChoiceAnswer3Energie1, Question = question3Energie, FlowSession = session1 };
        multipleChoiceAnswer3Energie1.Response = response3Energie1;

        var response3Energie2 = new Response
            { Answer = multipleChoiceAnswer3Energie2, Question = question3Energie, FlowSession = session1 };
        multipleChoiceAnswer3Energie2.Response = response3Energie2;

        var response3Energie3 = new Response
            { Answer = multipleChoiceAnswer3Energie3, Question = question3Energie, FlowSession = session1 };
        multipleChoiceAnswer3Energie3.Response = response3Energie3;

        var response4Energie = new Response
            { Answer = openAnswer4Energie, Question = question4Energie, FlowSession = session1 };
        openAnswer4Energie.Response = response4Energie;

        var response5GezondEten = new Response
            { Answer = openAnswer5GezondEten, Question = question5GezondEten, FlowSession = session1 };
        openAnswer5GezondEten.Response = response5GezondEten;

        var response6OpenbaarVervoer1 = new Response
        {
            Answer = multipleChoiceAnswer6OpenbaarVervoer1, Question = question6OpenbaarVervoer, FlowSession = session1
        };
        multipleChoiceAnswer6OpenbaarVervoer1.Response = response6OpenbaarVervoer1;

        var response6OpenbaarVervoer2 = new Response
        {
            Answer = multipleChoiceAnswer6OpenbaarVervoer2, Question = question6OpenbaarVervoer, FlowSession = session1
        };
        multipleChoiceAnswer6OpenbaarVervoer2.Response = response6OpenbaarVervoer2;

        var response6OpenbaarVervoer3 = new Response
        {
            Answer = multipleChoiceAnswer6OpenbaarVervoer3, Question = question6OpenbaarVervoer, FlowSession = session1
        };
        multipleChoiceAnswer6OpenbaarVervoer3.Response = response6OpenbaarVervoer3;


        context.AddRange(response1Mentaal1, response1Mentaal2, response1Mentaal3,
            response2Mentaal1, response2Mentaal2, response2Mentaal3,
            response3Energie1, response3Energie2, response3Energie3,
            response4Energie,
            response5GezondEten,
            response6OpenbaarVervoer1, response6OpenbaarVervoer2, response6OpenbaarVervoer3, responseConditionalPoint);

        #endregion

        context.SaveChanges();

        context.ChangeTracker.Clear();
    }
}