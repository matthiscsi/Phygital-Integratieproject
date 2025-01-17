export interface FlowStepDto {
    flowStepId: number;
    orderNr: number;
    flowStepType: string;
    subthemeId: number;
    subtheme: string;
    isActive: boolean;
    subthemeTitle: string;

}

export interface InfoDto extends FlowStepDto {
    flowStepType: "Info"; 
    body: string;
    uploadedImage: string;
    uploadedVideo: string;
    uploadedAudio: string;
    urlImage: string;
    urlVideo: string;
    urlAudio: string;
}

export interface QuestionDto extends FlowStepDto {
    query: string;
}

export interface MultipleChoiceQuestionDto extends QuestionDto {
    flowStepType: "MultipleChoiceQuestion"; 
    options: OptionDto[];
}

export interface OpenQuestionDto extends QuestionDto {
    flowStepType: "OpenQuestion"; 
}

export interface ClosedQuestionDto extends QuestionDto {
    flowStepType: "ClosedQuestion";
    options: OptionDto[];
}

export interface OptionDto {
    id: number;
    text: string;
}

export interface RangeQuestionDto extends QuestionDto {
    flowStepType: "RangeQuestion"; // Discriminant property
    minValue: number;
    maxValue: number;
}
export interface ThankYou extends FlowStepDto{
    flowStepType: "ThankYou";
}