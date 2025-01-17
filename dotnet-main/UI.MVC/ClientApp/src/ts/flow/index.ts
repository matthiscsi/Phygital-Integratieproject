let counter: number = 1;

document.addEventListener("DOMContentLoaded", function() {
    const counterElement: HTMLElement | null = document.getElementById("counter");
    const incrementButton: HTMLElement | null = document.getElementById("incrementBtn");
    const decrementButton: HTMLElement | null = document.getElementById("decrementBtn");

    incrementButton?.addEventListener("click", function() {
        if (counter < 4) {
            counter++;
            counterElement!.textContent = counter.toString();
        }
    });

    decrementButton?.addEventListener("click", function() {
        if (counter > 1) {
            counter--;
            counterElement!.textContent = counter.toString();
            console.log(counter);
        }
    });
});

export { counter };