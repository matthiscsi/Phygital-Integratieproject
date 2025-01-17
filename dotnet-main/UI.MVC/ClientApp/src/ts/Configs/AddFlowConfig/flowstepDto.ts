
export interface FlowStep {
 
    orderNr: number;
    header?: string;
    flow: number;
    subTheme: number;
}


export interface Question extends FlowStep {
    query: string;
}


export interface RangeQuestion extends Question {
    minValue: number;
    maxValue: number;
}


export interface OpenQuestion extends Question {}


export interface MultipleChoiceQuestion extends Question {
    options: Option[];
}


export interface ClosedQuestion extends Question {
    options: Option[];
}


export interface Option {
    text: string;
}

export interface Info extends FlowStep {
body: string;
image: string;
 video: string;
  audio: string;

}

export interface Note
{
    Title:string;
    Description: string;
    FlowsessionId: number;
}