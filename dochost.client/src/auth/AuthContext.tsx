import {createContext, useContext, useState} from "react";
import {ISession, LoginInput} from "../core/contracts/auth.ts";
import {login} from "../core/http/api.ts";

type IAuthContext = {
    session: ISession | null
    logIn: (formData: LoginInput, callback: () => void) => void
    logOut: (callback: () => void) => void
}

const AuthContext = createContext<IAuthContext>({
    logIn: () => {
    },
    logOut: () => {
    },
    session: null
});

const AuthProvider = ({children}) => {
    const [session, setSession] = useState<ISession | null>(null);

    const logIn = async (formData: LoginInput, callback: () => void) => {
        const res = await login(formData);
        if (res) {
            setSession({
                user: res.user,
                token: res.accessToken
            });
            sessionStorage.setItem("token", res.accessToken);
            callback();
            return;
        }
    }

    const logOut = (callback: () => void) => {
        setSession(null);
        sessionStorage.removeItem("token");
        callback();
    }

    return <AuthContext.Provider value={{
        logIn, logOut, session
    }}>
        {children}
    </AuthContext.Provider>
}

export default AuthProvider;

export const useAuth = () => {
    return useContext(AuthContext);
}