import {ReactElement, useEffect, useMemo, useState} from "react";
import {ISession, LoginInput} from "../core/contracts/auth.ts";
import {login, register} from "../core/http/api.ts";
import {attachToken} from "../core/http/axiosInstance.ts";
import { AuthContext } from "./useAuth.ts";

type AuthProviderProps = {
    children: ReactElement
}

const AuthProvider = ({children}: AuthProviderProps) => {
    const [session, setSession] = useState<ISession | null>(null);
    const [loading, setLoading] = useState<boolean>(false);
    const [loadingInitial, setLoadingInitial] = useState<boolean>(true);

    useEffect(() => {
        const token = sessionStorage.getItem("token");
        if (token) {
            setSession({
                user: session?.user || null,
                token,
            });
            attachToken(token);
        }
        setLoadingInitial(false);

    }, [session?.user]);

    const logIn = async (formData: LoginInput, callback: () => void) => {
        const res = await login(formData);
        if (res?.accessToken) {
            setSession({
                user: res.user,
                token: res.accessToken
            });
            attachToken(res.accessToken);
            sessionStorage.setItem("token", res.accessToken);
            setLoading(false);
            callback();
            return;
        }
    }

    const signUp = async (formData: LoginInput, callback: () => void) => {
        const res = await register(formData);
        if (res) {
            callback();
            return;
        }
    }

    const logOut = (callback: () => void) => {
        setSession(null);
        sessionStorage.removeItem("token");
        callback();
    }

    const authValue = useMemo(() => {
        return {
            logIn, logOut, session, loading, signUp
        }
    }, [session, loading]);

    return <AuthContext.Provider value={authValue}>
        {!loadingInitial && children}
    </AuthContext.Provider>
}

export default AuthProvider;
