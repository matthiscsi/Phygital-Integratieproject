// @ts-ignore
import {FlowStepDto, OptionDto, QuestionDto} from "./flowstepDto";
export interface ResponseDto {
    Question: QuestionDto;
    Answer: AnswerDto | null;
}

//nodig
export interface AnswerDto {
    answerType: string;
}


export interface MultipleChoiceAnswerDto extends AnswerDto {
    options: OptionDto[],
    selectedAnswers: number[]
}

export interface openAnswer extends AnswerDto {
    Answer: string;

}

export interface ClosedAnwser extends AnswerDto {
    selectedOption: number | null
}
export interface RangeAnswerDto extends AnswerRequest {
    SelectedValue: number;
}
export interface RangeAnswerDtoo extends AnswerDto {
    selectedAnswer: number;
}


export interface AnswerRequest {
    flowSessionId: number;

    // Question: QuestionDto;
}

export interface AnswerRequestOpenQuestion extends AnswerRequest {
    answer: string;

}

export interface AnswerRequestClosedQuestion extends AnswerRequest {

    SelectedAnswer: number;

}

export interface AnswerRequestMultipleChoiceQuestion extends AnswerRequest {

    SelectedAnswers: number[] | null;

}



