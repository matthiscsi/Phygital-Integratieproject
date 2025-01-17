function applySettings() {
    const settingsJson = localStorage.getItem('settings');
    const settings = settingsJson ? JSON.parse(settingsJson) : null;

    if (settings) {
        document.documentElement.style.setProperty('--font-family', settings.font);
        const fontElement = document.getElementById('font') as HTMLInputElement | null;
        if (fontElement) {
            fontElement.value = settings.font;
        }
    }
}

function saveSettings(event: Event) {
    event.preventDefault();
    const fontElement = document.getElementById('font') as HTMLInputElement | null;

    if (fontElement) {
        const font = fontElement.value;
        const settings = {
            font,
        };
        localStorage.setItem('settings', JSON.stringify(settings));
        applySettings();
    }
}

document.addEventListener('DOMContentLoaded', applySettings);

const btn = document.getElementById('btnSettings') as HTMLButtonElement | null;
if (btn) {
    btn.addEventListener('click', saveSettings);
}
