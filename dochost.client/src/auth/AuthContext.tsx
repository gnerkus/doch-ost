import {createContext, useContext, useState} from "react";
import {ISession, LoginInput} from "../core/contracts/auth.ts";
import {login} from "../core/api.ts";

type IAuthContext = {
    session: ISession | null
    logIn: (formData: LoginInput) => void
    logOut: () => void
}

const AuthContext = createContext<IAuthContext>({
    logIn: formData => {
    },
    logOut: () => {
    },
    session: null
});

const AuthProvider = ({children}) => {
    const [user, setUser] = useState(null);
    const [token, setToken] = useState(sessionStorage.getItem("token"));

    const logIn = async (formData: LoginInput) => {
        const res = await login(formData);
        if (res.data) {
            setUser(res.data.user);
            setToken(res.token);
            sessionStorage.setItem("token", res.token);
            // TODO: redirect to dashboard
            return;
        }
    }

    const logOut = () => {
        setUser(null);
        setToken(null);
        sessionStorage.removeItem("token");
        // TODO: redirect to login
    }

    return <AuthContext.Provider value={{
        logIn, logOut, session: {
            user, token
        }
    }}>
        {children}
    </AuthContext.Provider>
}

export default AuthProvider;

export const useAuth = () => {
    return useContext(AuthContext);
}