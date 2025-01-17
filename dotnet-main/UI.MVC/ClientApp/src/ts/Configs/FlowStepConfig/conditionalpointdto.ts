import {FlowStepDto, QuestionDto} from "./flowstepDto";
import {AnswerDto} from "./AddAnswersInterfaces";

export interface ConditionalPointDto {
    Question: QuestionDto,
    Criteria: AnswerDto;
    FollowUpStep: FlowStepDto;
}
