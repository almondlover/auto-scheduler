export interface Group{
    id: number,
    organizationId: number,
    name: string,
    description: string | undefined,
    subGroups: Group[]
};

export interface Member{
    id: number,
    name: string,
    contact: string | undefined
};

export interface Organization{
    id: number,
    name: string,
    description: string | undefined,
    groups: Group[]
};