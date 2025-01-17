export interface ClosedAnswerData {
    quantity: number;
    closedAnswerDto: ClosedAnswerDto
}
export interface ClosedAnswerDto {
    selectedAnswer: number;
    selectedAnswerText: string;
}
export interface MultipleChoiceAnswerData {
    quantity: number;
    multipleChoiceAnswerDto: MultipleChoiceAnswerDto
}

export interface MultipleChoiceAnswerDto {
    selectedAnswer: number;
    selectedAnswerText: string;
}

export interface RangeAnswerData {
    quantity: number;
    rangeAnswerDto: RangeAnswerDto
}
export interface RangeAnswerDto {
    selectedAnswer: number;
}