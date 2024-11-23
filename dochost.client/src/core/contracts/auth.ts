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

export type IAuthContext = {
    loading: boolean
    session: ISession | null
    logIn: (formData: LoginInput, callback: () => void) => void
    signUp: (formData: LoginInput, callback: () => void) => void
    logOut: (callback: () => void) => void
}