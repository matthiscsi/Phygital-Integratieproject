import {currentStepType} from "./flowdoorlopen";

document.addEventListener('DOMContentLoaded', function () {
    let isKeyPressed = false;
    let currentOptionIndex = -1; // Initialize to 0
    let nextBtn = document.getElementById('nextButton') as HTMLButtonElement;
    let skipBtn = document.getElementById('SkipButton') as HTMLButtonElement;

    function getElements() {
        return {
            multipleChoiceOptions: document.querySelectorAll('.multiple-choice-option') as NodeListOf<HTMLElement>,
            rangeInput: document.querySelector('.range-input') as HTMLInputElement,
            closedQuestionOptions: document.querySelectorAll('.closed-question-option') as NodeListOf<HTMLElement>
        };
    }

    enum QuestionType {
        MultipleChoice,
        Range,
        Closed,
        None
    }

    function getCurrentQuestionType(): QuestionType {
        if (currentStepType === "MultipleChoiceQuestion") {
            return QuestionType.MultipleChoice;
        } else if (currentStepType === "RangeQuestion") {
            return QuestionType.Range;
        } else if (currentStepType === "ClosedQuestion") {
            return QuestionType.Closed;
        } else {
            return QuestionType.None;
        }
    }

    function clickMultipleChoiceOption(index: number) {
        const {multipleChoiceOptions} = getElements();
        const option = multipleChoiceOptions[index];
        if (option) {
            //@ts-ignore
            option.checked = !option.checked; // Toggle the checked state
            option.focus(); // Focus on the input element
        }
    }


    function clickClosedQuestionOption(index: number) {
        const {closedQuestionOptions} = getElements();
        closedQuestionOptions.forEach((option, i) => {
            if (i === index) {
                //@ts-ignore
                option.checked = true; // Check the input element
                //@ts-ignore
                option.focus(); // Focus on the input element    cus on the input element
            } else {
                //@ts-ignore
                option.checked = false;  // Remove highlight from other options
            }
        });
    }

    function selectMultipleChoiceOption(index: number) {
        const {multipleChoiceOptions} = getElements();
        multipleChoiceOptions.forEach((option, i) => {
            if (i === index) {
                option.classList.add('selected');
                option.focus();
            }
        });
    }

    function selectClosedQuestionOption(index: number) {
        const {closedQuestionOptions} = getElements();
        closedQuestionOptions.forEach((option, i) => {
            if (i === index) {
                option.classList.add('selected');
                option.focus();
            } else {
                option.classList.remove('selected');
            }
        });
    }

    async function makeyMakeyInput(event: KeyboardEvent) {
        if (isKeyPressed) return; // If the key is already pressed, return

        let questionType = getCurrentQuestionType();
        console.log('Current Question Type:', questionType);
        const {rangeInput} = getElements();
        switch (event.key) {
            case 'ArrowDown':
                console.log('ArrowDown pressed');
                event.preventDefault();
                await nextBtn.click();
                break;
            case 'ArrowLeft':
                console.log('ArrowLeft pressed');
                
                event.preventDefault();
                if (questionType === QuestionType.MultipleChoice) {
                    clickMultipleChoiceOption(currentOptionIndex);
                } else if (questionType === QuestionType.Closed) {
                    clickClosedQuestionOption(currentOptionIndex);
                } else if (questionType === QuestionType.Range) {
                    if (rangeInput) {
                        rangeInput.value = (parseInt(rangeInput.value) - 1).toString();
                        document.getElementById('rangeValue')!.innerText = rangeInput.value;
                    }
                }
                break;
            case 'ArrowRight':
                console.log('ArrowRight pressed');
                event.preventDefault();
                if (questionType === QuestionType.MultipleChoice) {
                    const {multipleChoiceOptions} = getElements();
                    if (multipleChoiceOptions.length > 0) {
                        currentOptionIndex = (currentOptionIndex + 1) % multipleChoiceOptions.length;
                        selectMultipleChoiceOption(currentOptionIndex);
                    } else {
                        console.error("No multiple choice options available.");
                    }
                } else if (questionType === QuestionType.Closed) {
                    const {closedQuestionOptions} = getElements();
                    if (closedQuestionOptions.length > 0) {
                        currentOptionIndex = (currentOptionIndex + 1) % closedQuestionOptions.length;
                        selectClosedQuestionOption(currentOptionIndex);
                    } else {
                        console.error("No closed question options available.");
                    }
                } else if (questionType === QuestionType.Range) {
                    // @ts-ignore
                    if (rangeInput) {
                        rangeInput.value = (parseInt(rangeInput.value) + 1).toString();
                        document.getElementById('rangeValue')!.innerText = rangeInput.value;
                    }
                }
                break;
            case 'ArrowUp':
                console.log('ArrowUp pressed');
                event.preventDefault();
                await skipBtn.click();
                break;
            default:
                break;
        }

        isKeyPressed = true; // Mark that the key is pressed
    }

    function resetKeyPressed() {
        isKeyPressed = false; // Reset key pressed state on key up
    }

    document.addEventListener('keydown', makeyMakeyInput);
    document.addEventListener('keyup', resetKeyPressed);

    // Initial setup to highlight the first option if available
    const {multipleChoiceOptions, closedQuestionOptions} = getElements();
    if (multipleChoiceOptions.length > 0) {
        selectMultipleChoiceOption(currentOptionIndex);
    } else if (closedQuestionOptions.length > 0) {
        selectClosedQuestionOption(currentOptionIndex);
    }
});
