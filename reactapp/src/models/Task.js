export class Task {
    constructor(id, name, description, extraText) {
        this.name = name;
        this.description = description;
        this.id = id;
        this.extraText = extraText;
    }

    static fromDto(dto) {
        const id = dto.Id;
        const name = dto.Name;
        const description = dto.Description;
        const extraText = dto.Competences ? dto.Competences.reduce((accumulator, value) => accumulator  + value + "\n") : null;

        return new Task(id, name, description, extraText);
    }

    toDto = () => {
        const Category = 0;
        const Id = this.id;
        const Description = this.description;
        const Competences = this.extraText ? this.extraText.split(".\n") : [""];
        const Name = this.name;

        return {Category, Id, Description, Competences, Name};
    }
}