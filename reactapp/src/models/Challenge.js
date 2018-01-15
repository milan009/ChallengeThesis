import {Task} from "./Task";

export class Challenge {
    constructor(challenge) {
        this.names = {...challenge.names};
        this.description = challenge.description;
        this.requirements = {...challenge.requirements};
        this.tasks = challenge.tasks;
        this.id = challenge.id;
        this.category = challenge.category;
        this.imageUri = challenge.imageUri;
    }

    static fromDto(dto) {
        const id = dto.Id;
        const category = dto.Category;
        const names = { male: dto.Names[0].Name, female: dto.Names[1].Name };
        const description = dto.Description;
        const requirements = {
            basic: {
                cubs: dto.BasicRequirements.Cubs,
                scouts: dto.BasicRequirements.Scouts,
                guides: dto.BasicRequirements.Guides,
            },
            extra: {
                cubs: dto.ExtraRequirements.Cubs,
                scouts: dto.ExtraRequirements.Scouts,
                guides: dto.ExtraRequirements.Guides,
            },
        };
        const imageUri = dto.ImageUri.Uri;
        const tasks = {
            basic: dto.BasicTasks.map((tDto) => Task.fromDto(tDto)),
            extra: dto.ExtraTasks.map((tDto) => Task.fromDto(tDto)),
        };

        return new Challenge({id, names, description, category, requirements, tasks, imageUri});
    }

    toDto = () => {
        const BasicRequirements = {
            Cubs: this.requirements.basic.cubs,
            Scouts: this.requirements.basic.scouts,
            Guides: this.requirements.basic.guides,
        };
        const ExtraRequirements = {
            Cubs: this.requirements.extra.cubs,
            Scouts: this.requirements.extra.scouts,
            Guides: this.requirements.extra.guides,
        };
        const Category = this.category;
        const Names = [{Name: this.names.male, Gender: 0}, {Name: this.names.female, Gender: 1}];
        const Description = this.description;
        const ImageUri = {Uri: this.imageUri, LocalPath: null};
        const BasicTasks = this.tasks.basic.map((task) => task.toDto());
        const ExtraTasks = this.tasks.extra.map((task) => task.toDto());
        const Id = this.id;
        console.log(ImageUri);
        return {BasicRequirements, ExtraRequirements, Category, Names, Description, ImageUri, BasicTasks, ExtraTasks, Id};
    };
}