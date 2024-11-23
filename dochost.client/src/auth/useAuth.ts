import {createContext, useContext} from "react";
import {IAuthContext} from "../core/contracts/auth.ts";

export const AuthContext = createContext<IAuthContext>({
    logIn: () => {
    },
    logOut: () => {
    },
    signUp: () => {
    },
    session: null,
    loading: true
});

export const useAuth = () => {
    return useContext(AuthContext);
}