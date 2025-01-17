import {updateOrder} from "./updateorder";

let dragSrcEl: HTMLElement | null = null;
export function handleDragStart(this: any, e: any) {
    dragSrcEl = this;
    e.dataTransfer.effectAllowed = 'move';
    e.dataTransfer.setData('text/html', this.outerHTML);
}

export function handleDragOver(e: any) {
    if (e.preventDefault) {
        e.preventDefault();
    }
    e.dataTransfer.dropEffect = 'move';
    return false;
}

export function handleDragEnter(this: any, e: any) {
    this.classList.add('over');
}

export function handleDragLeave(this: any, e: any) {
    this.classList.remove('over');
}

export function handleDrop(this: any, e: any) {
    // Voorkom dat de browser de drop zelf afhandelt
    if (e.stopPropagation) {
        e.stopPropagation();
    }

    // Verplaats het draggable element naar het doelelement
    if (dragSrcEl !== this) {
        this.parentNode.removeChild(dragSrcEl);
        const dropHTML = e.dataTransfer.getData('text/html');
        this.insertAdjacentHTML('beforebegin', dropHTML);
        const dropElem = this.previousSibling;
        dropElem.addEventListener('dragstart', handleDragStart, false);
        dropElem.addEventListener('dragenter', handleDragEnter, false);
        dropElem.addEventListener('dragover', handleDragOver, false);
        dropElem.addEventListener('dragleave', handleDragLeave, false);
        dropElem.addEventListener('drop', handleDrop, false);
        dropElem.addEventListener('dragend', handleDragEnd, false);
    }
    this.classList.remove('over');

    updateOrder();
    return false;
}

export function handleDragEnd(this: any, e: any) {
    this.classList.remove('over');
}
