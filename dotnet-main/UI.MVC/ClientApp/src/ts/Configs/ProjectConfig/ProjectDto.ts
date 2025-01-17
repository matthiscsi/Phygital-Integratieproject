export interface ProjectDto {
    Circulair: boolean;
    projectId: number;
    name: string;
    description: string;
    isActive: boolean;
    backgroundColor: string;
    font: string;
}

export interface flowdto
{
    flowId : number;
    title : string;
}
export interface projectPost {
    projectId: number;
    Circulair: boolean;
}