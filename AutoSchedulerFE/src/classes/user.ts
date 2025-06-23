export interface LoginModel{
    email:string,
    password:string
};

export interface RegisterModel{
    userName:string,
    email:string,
    password:string,
    role:string
};

export interface User{
    email: string,
    username:string | undefined
}