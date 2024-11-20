export type IAuthUser = {
    email: string
}

export type ISession = {
    token: string | null
    user: IAuthUser | null
}

export type LoginInput = {
    email: string
    password: string
}